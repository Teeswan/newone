using EPMS.Application.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Enums;
using EPMS.Domain.Entities;
using Microsoft.Reporting.NETCore;
using System.Reflection;
using System.IO;
using System.Linq;

namespace EPMS.Infrastructure.Services;

public class RdlcReportService : IRdlcReportService
{
    private readonly IPerformanceEvaluationRepository _evaluationRepository;
    private readonly IAppraisalResponseRepository _responseRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppraisalCycleRepository _cycleRepository;
    private readonly IPerformanceOutcomeRepository _outcomeRepository;
    private readonly IRatingScaleRepository _ratingScaleRepository;

    public RdlcReportService(
        IPerformanceEvaluationRepository evaluationRepository,
        IAppraisalResponseRepository responseRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IAppraisalCycleRepository cycleRepository,
        IPerformanceOutcomeRepository outcomeRepository,
        IRatingScaleRepository ratingScaleRepository)
    {
        _evaluationRepository = evaluationRepository;
        _responseRepository = responseRepository;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _cycleRepository = cycleRepository;
        _outcomeRepository = outcomeRepository;
        _ratingScaleRepository = ratingScaleRepository;
    }

    public async Task<byte[]> GenerateEmployeePerformanceSummaryReportAsync(int employeeId, int cycleId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        var cycle = await _cycleRepository.GetByIdAsync(cycleId);
        var evals = await _evaluationRepository.GetByEmployeeIdAsync(employeeId);
        var eval = evals.FirstOrDefault(e => e.CycleId == cycleId && (e.IsFinalized == true || e.Status == PerformanceEvaluationStatus.Finalized));

        if (employee == null || cycle == null || eval == null)
        {
            throw new Exception("Required finalized evaluation data not found for this employee and cycle.");
        }

        var responses = await _responseRepository.GetByEvalIdAsync(eval.EvalId);
        var responsesDto = responses.Select(r => new AppraisalResponseDto
        {
            QuestionText = r.Question?.QuestionText,
            QuestionCategory = r.Question?.Category,
            RatingValue = r.RatingValue,
            AnswerText = r.AnswerText,
            RespondentRole = r.RespondentRole
        }).ToList();

        var scales = await _ratingScaleRepository.GetAllAsync();
        var band = scales.OrderByDescending(s => s.RatingLevel)
            .FirstOrDefault(s => (eval.FinalRatingScore ?? 0) >= s.RatingLevel);

        var reportData = new EmployeePerformanceSummaryReportDto
        {
            EmployeeCode = employee.EmployeeCode ?? "N/A",
            FullName = employee.FullName,
            PositionTitle = employee.Position?.PositionTitle ?? "N/A",
            LevelName = employee.Position?.LevelId ?? "N/A",
            DepartmentName = employee.Department?.DepartmentName ?? "N/A",
            ReviewPeriod = cycle.CycleName,
            ReviewStartDate = cycle.StartDate.ToDateTime(TimeOnly.MinValue),
            ReviewEndDate = cycle.EndDate.ToDateTime(TimeOnly.MinValue),
            TotalWeightedScore = eval.FinalRatingScore ?? 0,
            AchievementPercentage = eval.FinalRatingScore ?? 0,
            FinalRating = (eval.FinalRatingScore ?? 0) / 20, 
            PerformanceLevel = band?.Label ?? "N/A",
            PromotionEligibility = (eval.FinalRatingScore ?? 0) >= 90,
            HasActivePip = (eval.FinalRatingScore ?? 0) < 50,
            KpiCategoryScores = responsesDto.GroupBy(r => r.QuestionCategory)
                .Select(g => new KpiCategoryScoreDto
                {
                    CategoryName = g.Key ?? "General",
                    Score = (decimal)g.Average(x => x.RatingValue ?? 0) / 5 * 100
                }).ToList()
        };

        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("EmployeePerformanceSummary.rdlc");
            report.LoadReportDefinition(reportStream);
            report.DataSources.Add(new ReportDataSource("CategoryScores", reportData.KpiCategoryScores));
            
            report.SetParameters(new[]
            {
                new ReportParameter("EmployeeName", reportData.FullName),
                new ReportParameter("EmployeeCode", reportData.EmployeeCode),
                new ReportParameter("DepartmentName", reportData.DepartmentName),
                new ReportParameter("CycleName", reportData.ReviewPeriod),
                new ReportParameter("FinalScore", reportData.TotalWeightedScore.ToString("N2")),
                new ReportParameter("PerformanceLevel", reportData.PerformanceLevel)
            });

            return report.Render("PDF");
        });
    }

    public async Task<byte[]> GenerateDepartmentPerformanceComparisonReportAsync(int cycleId)
    {
        var depts = await _departmentRepository.GetAllAsync();
        var evals = await _evaluationRepository.GetByCycleIdAsync(cycleId);
        var finalizedEvals = evals.Where(e => e.IsFinalized == true || e.Status == PerformanceEvaluationStatus.Finalized).ToList();

        var deptComparison = depts.Select(d => new DepartmentPerformanceDto
        {
            DepartmentName = d.DepartmentName,
            EmployeeCount = finalizedEvals.Count(e => e.Employee?.DepartmentId == d.DepartmentId),
            AverageScore = (decimal)(finalizedEvals.Where(e => e.Employee?.DepartmentId == d.DepartmentId).Any() 
                ? finalizedEvals.Where(e => e.Employee?.DepartmentId == d.DepartmentId).Average(e => e.FinalRatingScore ?? 0) : 0),
            HighestScore = finalizedEvals.Where(e => e.Employee?.DepartmentId == d.DepartmentId).Any()
                ? finalizedEvals.Where(e => e.Employee?.DepartmentId == d.DepartmentId).Max(e => e.FinalRatingScore ?? 0) : 0,
            LowestScore = finalizedEvals.Where(e => e.Employee?.DepartmentId == d.DepartmentId).Any()
                ? finalizedEvals.Where(e => e.Employee?.DepartmentId == d.DepartmentId).Min(e => e.FinalRatingScore ?? 0) : 0
        }).ToList();

        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("DepartmentPerformanceComparison.rdlc");
            report.LoadReportDefinition(reportStream);
            report.DataSources.Add(new ReportDataSource("DepartmentData", deptComparison));

            return report.Render("PDF");
        });
    }

    public async Task<byte[]> GenerateHighLowPerformerReportAsync(int cycleId)
    {
        var evals = await _evaluationRepository.GetByCycleIdAsync(cycleId);
        var finalizedEvals = evals.Where(e => e.IsFinalized == true || e.Status == PerformanceEvaluationStatus.Finalized)
            .OrderByDescending(e => e.FinalRatingScore ?? 0)
            .ToList();

        var ranking = finalizedEvals.Select((e, index) => new PerformerRankingDto
        {
            Rank = index + 1,
            EmployeeCode = e.Employee?.EmployeeCode ?? "N/A",
            FullName = e.Employee?.FullName ?? "Unknown",
            DepartmentName = e.Employee?.Department?.DepartmentName ?? "N/A",
            PerformanceScore = e.FinalRatingScore ?? 0,
            Category = (index < finalizedEvals.Count * 0.1) ? "High Performer" : (index >= finalizedEvals.Count * 0.9 ? "Low Performer" : "Average")
        }).Where(r => r.Category != "Average").ToList();

        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("HighLowPerformerReport.rdlc");
            report.LoadReportDefinition(reportStream);
            report.DataSources.Add(new ReportDataSource("RankingData", ranking));

            return report.Render("PDF");
        });
    }

    public async Task<byte[]> GeneratePromotionIncrementRecommendationReportAsync(int cycleId)
    {
        var outcomes = await _outcomeRepository.GetAllAsync();
        var cycleOutcomes = outcomes.Where(o => o.CycleId == cycleId && (o.RecommendationType == "Promotion" || o.RecommendationType == "Salary Increment")).ToList();

        var recommendations = cycleOutcomes.Select(o => new PromotionRecommendationDto
        {
            EmployeeCode = o.Employee?.EmployeeCode ?? "N/A",
            FullName = o.Employee?.FullName ?? "Unknown",
            CurrentPosition = o.OldPosition?.PositionTitle ?? "N/A",
            RecommendedPosition = o.NewPosition?.PositionTitle ?? "N/A",
            RecommendationType = o.RecommendationType ?? "N/A",
            Justification = "Based on performance evaluation results."
        }).ToList();

        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream("PromotionIncrementRecommendation.rdlc");
            report.LoadReportDefinition(reportStream);
            report.DataSources.Add(new ReportDataSource("RecommendationData", recommendations));

            return report.Render("PDF");
        });
    }

    public async Task<byte[]> Generate360FeedbackReportAsync(AppraisalReportDto reportData)
    {
        return await GenerateReportAsync("Feedback360.rdlc", reportData);
    }

    public async Task<byte[]> GeneratePerformanceAppraisalReportAsync(AppraisalReportDto reportData)
    {
        return await GenerateReportAsync("PerformanceAppraisal.rdlc", reportData);
    }

    public async Task<byte[]> GenerateSelfAssessmentReportAsync(AppraisalReportDto reportData)
    {
        return await GenerateReportAsync("SelfAssessment.rdlc", reportData);
    }

    private async Task<byte[]> GenerateReportAsync(string reportName, AppraisalReportDto reportData)
    {
        return await Task.Run(() =>
        {
            var report = new LocalReport();
            using var reportStream = GetReportStream(reportName);
            report.LoadReportDefinition(reportStream);

            report.DataSources.Add(new ReportDataSource("Responses", reportData.Responses));

            report.SetParameters(new[]
            {
                new ReportParameter("EmployeeName", reportData.EmployeeName ?? ""),
                new ReportParameter("EmployeeCode", reportData.EmployeeCode ?? ""),
                new ReportParameter("DepartmentName", reportData.DepartmentName ?? ""),
                new ReportParameter("PositionTitle", reportData.PositionTitle ?? ""),
                new ReportParameter("ManagerName", reportData.ManagerName ?? ""),
                new ReportParameter("CycleName", reportData.CycleName ?? ""),
                new ReportParameter("AssessmentDate", reportData.AssessmentDate?.ToString("d") ?? ""),
                new ReportParameter("EffectiveDate", reportData.EffectiveDate?.ToString("d") ?? ""),
                new ReportParameter("FinalScore", reportData.FinalScore?.ToString("N2") ?? "0.00"),
                new ReportParameter("PerformanceBand", reportData.PerformanceBand ?? ""),
                new ReportParameter("TotalPoints", reportData.TotalPoints.ToString() ?? "0"),
                new ReportParameter("AnsweredQuestionsCount", reportData.AnsweredQuestionsCount.ToString() ?? "0"),
                new ReportParameter("MaxPoints", reportData.MaxPoints.ToString() ?? "0")
            });

            return report.Render("PDF");
        });
    }

    private Stream GetReportStream(string reportFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"EPMS.Infrastructure.Reports.{reportFileName}";
        
        var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Report file not found: {reportFileName}");
        }
        
        return stream;
    }
}
