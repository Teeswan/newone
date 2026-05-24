using ClosedXML.Excel;
using EPMS.Application.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EPMS.Infrastructure.Services;

public class ExcelPdfService : IExcelPdfService
{
    private readonly IAppraisalCycleService _cycleService;
    private readonly IAppraisalQuestionService _questionService;
    private readonly IAppraisalResponseService _responseService;
    private readonly IAppraisalFormService _formService;
    private readonly IFormQuestionService _formQuestionService;
    private readonly IPerformanceEvaluationService _evaluationService;
    private readonly IPerformanceOutcomeService _outcomeService;
    private readonly IDepartmentService _departmentService;
    private readonly ITeamService _teamService;
    private readonly IEmployeeService _employeeService;

    public ExcelPdfService(
        IAppraisalCycleService cycleService,
        IAppraisalQuestionService questionService,
        IAppraisalResponseService responseService,
        IAppraisalFormService formService,
        IFormQuestionService formQuestionService,
        IPerformanceEvaluationService evaluationService,
        IPerformanceOutcomeService outcomeService,
        IDepartmentService departmentService,
        ITeamService teamService,
        IEmployeeService employeeService)
    {
        _cycleService = cycleService;
        _questionService = questionService;
        _responseService = responseService;
        _formService = formService;
        _formQuestionService = formQuestionService;
        _evaluationService = evaluationService;
        _outcomeService = outcomeService;
        _departmentService = departmentService;
        _teamService = teamService;
        _employeeService = employeeService;
    }

    public async Task<byte[]> ExportAppraisalCyclesToExcelAsync()
    {
        var data = await _cycleService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("AppraisalCycles");

        ws.Cell(1, 1).Value = "CycleId";
        ws.Cell(1, 2).Value = "CycleName";
        ws.Cell(1, 3).Value = "StartDate";
        ws.Cell(1, 4).Value = "EndDate";
        ws.Cell(1, 5).Value = "EvaluationPeriod";
        ws.Cell(1, 6).Value = "CycleStatus";
        StyleHeaderRow(ws, 6);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.CycleId;
            ws.Cell(row, 2).Value = item.CycleName;
            ws.Cell(row, 3).Value = item.StartDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 4).Value = item.EndDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 5).Value = item.EvaluationPeriod ?? "";
            ws.Cell(row, 6).Value = item.CycleStatus ?? "";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportAppraisalQuestionsToExcelAsync()
    {
        var data = await _questionService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("AppraisalQuestions");

        ws.Cell(1, 1).Value = "QuestionId";
        ws.Cell(1, 2).Value = "QuestionText";
        ws.Cell(1, 3).Value = "Category";
        ws.Cell(1, 4).Value = "IsRequired";
        StyleHeaderRow(ws, 4);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.QuestionId;
            ws.Cell(row, 2).Value = item.QuestionText;
            ws.Cell(row, 3).Value = item.Category ?? "";
            ws.Cell(row, 4).Value = item.IsRequired == true ? "Yes" : "No";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportAppraisalResponsesToExcelAsync()
    {
        var data = await _responseService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("AppraisalResponses");

        ws.Cell(1, 1).Value = "ResponseId";
        ws.Cell(1, 2).Value = "EvalId";
        ws.Cell(1, 3).Value = "QuestionId";
        ws.Cell(1, 4).Value = "RespondentId";
        ws.Cell(1, 5).Value = "AnswerText";
        ws.Cell(1, 6).Value = "RatingValue";
        ws.Cell(1, 7).Value = "IsAnonymous";
        StyleHeaderRow(ws, 7);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.ResponseId;
            ws.Cell(row, 2).Value = item.EvalId ?? 0;
            ws.Cell(row, 3).Value = item.QuestionId ?? 0;
            ws.Cell(row, 4).Value = item.RespondentId ?? 0;
            ws.Cell(row, 5).Value = item.AnswerText ?? "";
            ws.Cell(row, 6).Value = item.RatingValue ?? 0;
            ws.Cell(row, 7).Value = item.IsAnonymous == true ? "Yes" : "No";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportAppraisalFormsToExcelAsync()
    {
        var data = await _formService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("AppraisalForms");

        ws.Cell(1, 1).Value = "FormId";
        ws.Cell(1, 2).Value = "FormName";
        ws.Cell(1, 3).Value = "IsActive";
        StyleHeaderRow(ws, 3);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.FormId;
            ws.Cell(row, 2).Value = item.FormName ?? "";
            ws.Cell(row, 3).Value = item.IsActive == true ? "Yes" : "No";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportFormQuestionsToExcelAsync()
    {
        var data = await _formQuestionService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("FormQuestions");

        ws.Cell(1, 1).Value = "FormId";
        ws.Cell(1, 2).Value = "QuestionId";
        ws.Cell(1, 3).Value = "SortOrder";
        StyleHeaderRow(ws, 3);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.FormId ?? 0;
            ws.Cell(row, 2).Value = item.QuestionId ?? 0;
            ws.Cell(row, 3).Value = item.SortOrder ?? 0;
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportPerformanceEvaluationsToExcelAsync()
    {
        var data = await _evaluationService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("PerformanceEvaluations");

        ws.Cell(1, 1).Value = "EvalId";
        ws.Cell(1, 2).Value = "EmployeeId";
        ws.Cell(1, 3).Value = "CycleId";
        ws.Cell(1, 4).Value = "SelfRating";
        ws.Cell(1, 5).Value = "ManagerRating";
        ws.Cell(1, 6).Value = "SelfComments";
        ws.Cell(1, 7).Value = "ManagerComments";
        ws.Cell(1, 8).Value = "FinalRatingScore";
        ws.Cell(1, 9).Value = "IsFinalized";
        ws.Cell(1, 10).Value = "FinalizedAt";
        StyleHeaderRow(ws, 10);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.EvalId;
            ws.Cell(row, 2).Value = item.EmployeeId ?? 0;
            ws.Cell(row, 3).Value = item.CycleId ?? 0;
            ws.Cell(row, 4).Value = item.SelfRating ?? 0;
            ws.Cell(row, 5).Value = item.ManagerRating ?? 0;
            ws.Cell(row, 6).Value = item.SelfComments ?? "";
            ws.Cell(row, 7).Value = item.ManagerComments ?? "";
            ws.Cell(row, 8).Value = item.FinalRatingScore ?? 0;
            ws.Cell(row, 9).Value = item.IsFinalized == true ? "Yes" : "No";
            ws.Cell(row, 10).Value = item.FinalizedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportPerformanceOutcomesToExcelAsync()
    {
        var data = await _outcomeService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("PerformanceOutcomes");

        ws.Cell(1, 1).Value = "OutcomeId";
        ws.Cell(1, 2).Value = "EvalId";
        ws.Cell(1, 3).Value = "EmployeeId";
        ws.Cell(1, 4).Value = "CycleId";
        ws.Cell(1, 5).Value = "RecommendationType";
        ws.Cell(1, 6).Value = "OldPositionId";
        ws.Cell(1, 7).Value = "NewPositionId";
        ws.Cell(1, 8).Value = "OldLevelId";
        ws.Cell(1, 9).Value = "NewLevelId";
        ws.Cell(1, 10).Value = "ApprovalStatus";
        ws.Cell(1, 11).Value = "EffectiveDate";
        StyleHeaderRow(ws, 11);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.OutcomeId;
            ws.Cell(row, 2).Value = item.EvalId ?? 0;
            ws.Cell(row, 3).Value = item.EmployeeId ?? 0;
            ws.Cell(row, 4).Value = item.CycleId ?? 0;
            ws.Cell(row, 5).Value = item.RecommendationType ?? "";
            ws.Cell(row, 6).Value = item.OldPositionId ?? 0;
            ws.Cell(row, 7).Value = item.NewPositionId ?? 0;
            ws.Cell(row, 8).Value = item.OldLevelId ?? "";
            ws.Cell(row, 9).Value = item.NewLevelId ?? "";
            ws.Cell(row, 10).Value = item.ApprovalStatus ?? "";
            ws.Cell(row, 11).Value = item.EffectiveDate?.ToString("yyyy-MM-dd") ?? "";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportDepartmentsToExcelAsync()
    {
        var data = await _departmentService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Departments");

        ws.Cell(1, 1).Value = "DepartmentId";
        ws.Cell(1, 2).Value = "DepartmentName";
        ws.Cell(1, 3).Value = "ParentDepartmentId";
        ws.Cell(1, 4).Value = "IsActive";
        StyleHeaderRow(ws, 4);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.DepartmentId;
            ws.Cell(row, 2).Value = item.DepartmentName;
            ws.Cell(row, 3).Value = item.ParentDepartmentId ?? 0;
            ws.Cell(row, 4).Value = item.IsActive ? "Yes" : "No";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportTeamsToExcelAsync()
    {
        var data = await _teamService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Teams");

        ws.Cell(1, 1).Value = "TeamId";
        ws.Cell(1, 2).Value = "TeamName";
        ws.Cell(1, 3).Value = "ManagerId";
        ws.Cell(1, 4).Value = "DepartmentId";
        StyleHeaderRow(ws, 4);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.TeamId;
            ws.Cell(row, 2).Value = item.TeamName;
            ws.Cell(row, 3).Value = item.ManagerId ?? 0;
            ws.Cell(row, 4).Value = item.DepartmentId ?? 0;
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportEmployeesToExcelAsync()
    {
        var data = await _employeeService.GetAllAsync();
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Employees");

        ws.Cell(1, 1).Value = "EmployeeCode";
        ws.Cell(1, 2).Value = "FullName";
        ws.Cell(1, 3).Value = "Email";
        ws.Cell(1, 4).Value = "Phone";
        ws.Cell(1, 5).Value = "DepartmentId";
        ws.Cell(1, 6).Value = "PositionId";
        ws.Cell(1, 7).Value = "ReportsTo";
        ws.Cell(1, 8).Value = "JoinDate";
        ws.Cell(1, 9).Value = "EmploymentStatus";
        ws.Cell(1, 10).Value = "IsActive";
        StyleHeaderRow(ws, 10);

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.EmployeeCode;
            ws.Cell(row, 2).Value = item.FullName;
            ws.Cell(row, 3).Value = item.Email ?? "";
            ws.Cell(row, 4).Value = item.Phone ?? "";
            ws.Cell(row, 5).Value = item.DepartmentId ?? 0;
            ws.Cell(row, 6).Value = item.PositionId ?? 0;
            ws.Cell(row, 7).Value = item.ReportsTo ?? 0;
            ws.Cell(row, 8).Value = item.JoinDate?.ToString("yyyy-MM-dd") ?? "";
            ws.Cell(row, 9).Value = item.EmploymentStatus ?? "";
            ws.Cell(row, 10).Value = (item.IsActive ?? true) ? "Yes" : "No";
            row++;
        }

        ws.Columns().AdjustToContents();
        return WorkbookToBytes(workbook);
    }

    public async Task<int> ImportAppraisalCyclesFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        int count = 0;

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var request = new CreateAppraisalCycleRequest
            {
                CycleName = row.Cell(2).GetString(),
                StartDate = DateOnly.Parse(row.Cell(3).GetString()),
                EndDate = DateOnly.Parse(row.Cell(4).GetString()),
                EvaluationPeriod = row.Cell(5).GetString(),
                CycleStatus = row.Cell(6).GetString()
            };
            await _cycleService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportAppraisalQuestionsFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        int count = 0;

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var request = new CreateAppraisalQuestionRequest
            {
                QuestionText = row.Cell(2).GetString(),
                Category = row.Cell(3).GetString(),
                IsRequired = row.Cell(4).GetString().Equals("Yes", StringComparison.OrdinalIgnoreCase)
            };
            await _questionService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportAppraisalResponsesFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        int count = 0;

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var request = new CreateAppraisalResponseRequest
            {
                EvalId = ParseNullableInt(row.Cell(2).GetString()),
                QuestionId = ParseNullableInt(row.Cell(3).GetString()),
                RespondentId = ParseNullableInt(row.Cell(4).GetString()),
                AnswerText = row.Cell(5).GetString(),
                RatingValue = ParseNullableInt(row.Cell(6).GetString()),
                IsAnonymous = row.Cell(7).GetString().Equals("Yes", StringComparison.OrdinalIgnoreCase)
            };
            await _responseService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportAppraisalFormsFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        int count = 0;

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var request = new CreateAppraisalFormRequest
            {
                FormName = row.Cell(2).GetString(),
                IsActive = row.Cell(3).GetString().Equals("Yes", StringComparison.OrdinalIgnoreCase)
            };
            await _formService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportFormQuestionsFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        int count = 0;

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var request = new CreateFormQuestionRequest
            {
                FormId = ParseNullableInt(row.Cell(1).GetString()),
                QuestionId = ParseNullableInt(row.Cell(2).GetString()),
                SortOrder = ParseNullableInt(row.Cell(3).GetString())
            };
            await _formQuestionService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportPerformanceEvaluationsFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        int count = 0;

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var request = new CreatePerformanceEvaluationRequest
            {
                EmployeeId = ParseNullableInt(row.Cell(2).GetString()),
                CycleId = ParseNullableInt(row.Cell(3).GetString()),
                SelfRating = ParseNullableInt(row.Cell(4).GetString()),
                ManagerRating = ParseNullableInt(row.Cell(5).GetString()),
                SelfComments = row.Cell(6).GetString(),
                ManagerComments = row.Cell(7).GetString(),
                FinalRatingScore = ParseNullableDecimal(row.Cell(8).GetString()),
                IsFinalized = row.Cell(9).GetString().Equals("Yes", StringComparison.OrdinalIgnoreCase),
                FinalizedAt = ParseNullableDateTime(row.Cell(10).GetString())
            };
            await _evaluationService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportPerformanceOutcomesFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        int count = 0;

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var request = new CreatePerformanceOutcomeRequest
            {
                EvalId = ParseNullableInt(row.Cell(2).GetString()),
                EmployeeId = ParseNullableInt(row.Cell(3).GetString()),
                CycleId = ParseNullableInt(row.Cell(4).GetString()),
                RecommendationType = row.Cell(5).GetString(),
                OldPositionId = ParseNullableInt(row.Cell(6).GetString()),
                NewPositionId = ParseNullableInt(row.Cell(7).GetString()),
                OldLevelId = row.Cell(8).GetString(),
                NewLevelId = row.Cell(9).GetString(),
                ApprovalStatus = row.Cell(10).GetString(),
                EffectiveDate = ParseNullableDateOnly(row.Cell(11).GetString())
            };
            await _outcomeService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportDepartmentsFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false)
    {
        using var workbook = new XLWorkbook(fileStream);
        IXLWorksheet ws;

        if (string.IsNullOrWhiteSpace(sheetName))
        {
            ws = workbook.Worksheets.First();
        }
        else
        {
            if (!workbook.Worksheets.TryGetWorksheet(sheetName, out var foundWs))
            {
                throw new ArgumentException($"Worksheet '{sheetName}' does not exist in this Excel file!");
            }
            ws = foundWs;
        }

        var expectedColumns = new[] { "DepartmentId", "DepartmentName", "ParentDepartmentId", "IsActive" };
        var headerRow = ws.Row(1);
        for (int i = 0; i < expectedColumns.Length; i++)
        {
            var cellValue = headerRow.Cell(i + 1).GetString();
            if (string.IsNullOrWhiteSpace(cellValue) || !cellValue.Equals(expectedColumns[i], StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid Excel file! Expected column {i + 1} to be '{expectedColumns[i]}', but found '{cellValue}'!");
            }
        }

        int count = 0;
        int rowsToSkip = skipFirstRow ? 1 : 0;
        var existingDepartments = skipExisting ? (await _departmentService.GetAllAsync()).ToList() : new List<DepartmentDto>();

        foreach (var row in ws.RowsUsed().Skip(rowsToSkip))
        {
            var deptName = row.Cell(2).GetString();
            
            if (string.IsNullOrWhiteSpace(deptName))
            {
                continue;
            }
            
            if (skipExisting && existingDepartments.Any(d => d.DepartmentName.Equals(deptName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var request = new CreateDepartmentRequest
            {
                DepartmentName = deptName,
                ParentDepartmentId = ParseNullableInt(row.Cell(3).GetString()),
                IsActive = row.Cell(4).GetString().Equals("Yes", StringComparison.OrdinalIgnoreCase)
            };
            await _departmentService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportTeamsFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false)
    {
        using var workbook = new XLWorkbook(fileStream);
        IXLWorksheet ws;

        if (string.IsNullOrWhiteSpace(sheetName))
        {
            ws = workbook.Worksheets.First();
        }
        else
        {
            if (!workbook.Worksheets.TryGetWorksheet(sheetName, out var foundWs))
            {
                throw new ArgumentException($"Worksheet '{sheetName}' does not exist in this Excel file!");
            }
            ws = foundWs;
        }

        var expectedColumns = new[] { "TeamId", "TeamName", "ManagerId", "DepartmentId" };
        var headerRow = ws.Row(1);
        for (int i = 0; i < expectedColumns.Length; i++)
        {
            var cellValue = headerRow.Cell(i + 1).GetString();
            if (string.IsNullOrWhiteSpace(cellValue) || !cellValue.Equals(expectedColumns[i], StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid Excel file! Expected column {i + 1} to be '{expectedColumns[i]}', but found '{cellValue}'!");
            }
        }

        int count = 0;
        int rowsToSkip = skipFirstRow ? 1 : 0;
        var existingTeams = skipExisting ? (await _teamService.GetAllAsync()).ToList() : new List<TeamDto>();

        foreach (var row in ws.RowsUsed().Skip(rowsToSkip))
        {
            var teamName = row.Cell(2).GetString();
            
            if (string.IsNullOrWhiteSpace(teamName))
            {
                continue;
            }
            
            if (skipExisting && existingTeams.Any(t => t.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var request = new CreateTeamRequest
            {
                TeamName = teamName,
                ManagerId = ParseNullableInt(row.Cell(3).GetString()),
                DepartmentId = ParseNullableInt(row.Cell(4).GetString())
            };
            await _teamService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<int> ImportEmployeesFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false)
    {
        using var workbook = new XLWorkbook(fileStream);
        IXLWorksheet ws;

        if (string.IsNullOrWhiteSpace(sheetName))
        {
            ws = workbook.Worksheets.First();
        }
        else
        {
            if (!workbook.Worksheets.TryGetWorksheet(sheetName, out var foundWs))
            {
                throw new ArgumentException($"Worksheet '{sheetName}' does not exist in this Excel file!");
            }
            ws = foundWs;
        }

        var expectedColumns = new[] { "EmployeeCode", "FullName", "Email", "Phone", "DepartmentId", "PositionId", "ReportsTo", "JoinDate", "EmploymentStatus", "IsActive" };
        var headerRow = ws.Row(1);
        for (int i = 0; i < expectedColumns.Length; i++)
        {
            var cellValue = headerRow.Cell(i + 1).GetString();
            if (string.IsNullOrWhiteSpace(cellValue) || !cellValue.Equals(expectedColumns[i], StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid Excel file! Expected column {i + 1} to be '{expectedColumns[i]}', but found '{cellValue}'!");
            }
        }

        int count = 0;
        int rowsToSkip = skipFirstRow ? 1 : 0;
        var existingEmployees = skipExisting ? (await _employeeService.GetAllAsync()).ToList() : new List<EmployeeDto>();

        foreach (var row in ws.RowsUsed().Skip(rowsToSkip))
        {
            var code = row.Cell(1).GetString();
            if (string.IsNullOrWhiteSpace(code)) continue;

            if (skipExisting && existingEmployees.Any(e => e.EmployeeCode.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var request = new CreateEmployeeRequest
            {
                EmployeeCode = code,
                FullName = row.Cell(2).GetString(),
                Email = row.Cell(3).GetString(),
                Phone = row.Cell(4).GetString(),
                DepartmentId = ParseNullableInt(row.Cell(5).GetString()),
                PositionId = ParseNullableInt(row.Cell(6).GetString()),
                ReportsTo = ParseNullableInt(row.Cell(7).GetString()),
                JoinDate = ParseNullableDateTime(row.Cell(8).GetString()),
                EmploymentStatus = row.Cell(9).GetString(),
                IsActive = row.Cell(10).GetString().Equals("Yes", StringComparison.OrdinalIgnoreCase) || row.Cell(10).GetString().Equals("True", StringComparison.OrdinalIgnoreCase)
            };
            await _employeeService.CreateAsync(request);
            count++;
        }

        return count;
    }

    public async Task<IEnumerable<PositionKpiImportDto>> ImportPositionKpiFromExcelAsync(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var ws = workbook.Worksheets.First();
        var list = new List<PositionKpiImportDto>();

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var kpiName = row.Cell(1).GetString();
            if (string.IsNullOrWhiteSpace(kpiName)) continue;

            var dto = new PositionKpiImportDto(
                KpiName: kpiName,
                Category: row.Cell(2).GetString(),
                Unit: row.Cell(3).GetString(),
                WeightPercent: ParseNullableDecimal(row.Cell(4).GetString()) ?? 0,
                TargetValue: ParseNullableDecimal(row.Cell(5).GetString()),
                PriorityLevel: Enum.TryParse<PriorityLevel>(row.Cell(6).GetString(), true, out var priority) ? priority : PriorityLevel.Medium,
                Direction: Enum.TryParse<KpiDirection>(row.Cell(7).GetString(), true, out var direction) ? direction : KpiDirection.HigherIsBetter,
                PositionId: ParseNullableInt(row.Cell(8).GetString())
            );
            list.Add(dto);
        }

        return list;
    }

    public async Task<byte[]> ExportPositionKpiTemplateAsync()
    {
        return await Task.Run(() => {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Position_KPI_Import_Template");

            ws.Cell(1, 1).Value = "KpiName";
            ws.Cell(1, 2).Value = "Category";
            ws.Cell(1, 3).Value = "Unit";
            ws.Cell(1, 4).Value = "WeightPercent";
            ws.Cell(1, 5).Value = "TargetValue";
            ws.Cell(1, 6).Value = "PriorityLevel (Critical/High/Medium/Lower)";
            ws.Cell(1, 7).Value = "Direction (HigherIsBetter/LowerIsBetter)";
            ws.Cell(1, 8).Value = "PositionId (Optional)";

            StyleHeaderRow(ws, 8);

            // Add a sample row
            ws.Cell(2, 1).Value = "Sample KPI";
            ws.Cell(2, 2).Value = "Quality";
            ws.Cell(2, 3).Value = "Percentage";
            ws.Cell(2, 4).Value = 10;
            ws.Cell(2, 5).Value = 100;
            ws.Cell(2, 6).Value = "Medium";
            ws.Cell(2, 7).Value = "HigherIsBetter";
            ws.Cell(2, 8).Value = "";

            ws.Columns().AdjustToContents();
            return WorkbookToBytes(workbook);
        });
    }

    public async Task<IEnumerable<EmployeeKpiImportDto>> ImportEmployeeKpiFromExcelAsync(Stream fileStream)
    {
        return await Task.Run(() => {
            using var workbook = new XLWorkbook(fileStream);
            var ws = workbook.Worksheets.First();
            var list = new List<EmployeeKpiImportDto>();

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var empIdStr = row.Cell(1).GetString();
                if (string.IsNullOrWhiteSpace(empIdStr)) continue;

                var dto = new EmployeeKpiImportDto(
                    EmployeeId: ParseNullableInt(empIdStr) ?? 0,
                    CycleId: ParseNullableInt(row.Cell(2).GetString()) ?? 0,
                    KpiName: row.Cell(3).GetString(),
                    Category: row.Cell(4).GetString(),
                    Unit: row.Cell(5).GetString(),
                    WeightPercent: ParseNullableDecimal(row.Cell(6).GetString()) ?? 0,
                    TargetValue: ParseNullableDecimal(row.Cell(7).GetString()) ?? 0,
                    Direction: Enum.TryParse<KpiDirection>(row.Cell(8).GetString(), true, out var direction) ? direction : KpiDirection.HigherIsBetter
                );
                list.Add(dto);
            }

            return (IEnumerable<EmployeeKpiImportDto>)list;
        });
    }

    public async Task<byte[]> ExportEmployeeKpiTemplateAsync()
    {
        return await Task.Run(() => {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Employee_KPI_Import_Template");

            ws.Cell(1, 1).Value = "EmployeeId";
            ws.Cell(1, 2).Value = "CycleId";
            ws.Cell(1, 3).Value = "KpiName";
            ws.Cell(1, 4).Value = "Category";
            ws.Cell(1, 5).Value = "Unit";
            ws.Cell(1, 6).Value = "WeightPercent";
            ws.Cell(1, 7).Value = "TargetValue";
            ws.Cell(1, 8).Value = "Direction (HigherIsBetter/LowerIsBetter)";

            StyleHeaderRow(ws, 8);

            // Add a sample row
            ws.Cell(2, 1).Value = 1;
            ws.Cell(2, 2).Value = 1;
            ws.Cell(2, 3).Value = "Individual Contribution";
            ws.Cell(2, 4).Value = "Behavioral";
            ws.Cell(2, 5).Value = "Rating";
            ws.Cell(2, 6).Value = 10;
            ws.Cell(2, 7).Value = 100;
            ws.Cell(2, 8).Value = "HigherIsBetter";

            ws.Columns().AdjustToContents();
            return WorkbookToBytes(workbook);
        });
    }

    public async Task<byte[]> ExportAppraisalCyclesToPdfAsync()
    {
        var data = (await _cycleService.GetAllAsync()).ToList();
        return GeneratePdf("Appraisal Cycles Report", new[] { "CycleId", "CycleName", "StartDate", "EndDate", "EvaluationPeriod", "CycleStatus" },
            data.Select(i => new[] { i.CycleId.ToString(), i.CycleName, i.StartDate.ToString("yyyy-MM-dd"), i.EndDate.ToString("yyyy-MM-dd"), i.EvaluationPeriod ?? "", i.CycleStatus ?? "" }).ToList());
    }

    public async Task<byte[]> ExportAppraisalQuestionsToPdfAsync()
    {
        var data = (await _questionService.GetAllAsync()).ToList();
        return GeneratePdf("Appraisal Questions Report", new[] { "QuestionId", "QuestionText", "Category", "IsRequired" },
            data.Select(i => new[] { i.QuestionId.ToString(), i.QuestionText, i.Category ?? "", i.IsRequired == true ? "Yes" : "No" }).ToList());
    }

    public async Task<byte[]> ExportAppraisalResponsesToPdfAsync()
    {
        var data = (await _responseService.GetAllAsync()).ToList();
        return GeneratePdf("Appraisal Responses Report", new[] { "ResponseId", "EvalId", "QuestionId", "RespondentId", "AnswerText", "RatingValue", "IsAnonymous" },
            data.Select(i => new[] { i.ResponseId.ToString(), (i.EvalId ?? 0).ToString(), (i.QuestionId ?? 0).ToString(), (i.RespondentId ?? 0).ToString(), i.AnswerText ?? "", (i.RatingValue ?? 0).ToString(), i.IsAnonymous == true ? "Yes" : "No" }).ToList());
    }

    public async Task<byte[]> ExportAppraisalFormsToPdfAsync()
    {
        var data = (await _formService.GetAllAsync()).ToList();
        return GeneratePdf("Appraisal Forms Report", new[] { "FormId", "FormName", "IsActive" },
            data.Select(i => new[] { i.FormId.ToString(), i.FormName ?? "", i.IsActive == true ? "Yes" : "No" }).ToList());
    }

    public async Task<byte[]> ExportFormQuestionsToPdfAsync()
    {
        var data = (await _formQuestionService.GetAllAsync()).ToList();
        return GeneratePdf("Form Questions Report", new[] { "FormId", "QuestionId", "SortOrder" },
            data.Select(i => new[] { (i.FormId ?? 0).ToString(), (i.QuestionId ?? 0).ToString(), (i.SortOrder ?? 0).ToString() }).ToList());
    }

    public async Task<byte[]> ExportPerformanceEvaluationsToPdfAsync()
    {
        var data = (await _evaluationService.GetAllAsync()).ToList();
        return GeneratePdf("Performance Evaluations Report",
            new[] { "EvalId", "EmployeeId", "CycleId", "SelfRating", "ManagerRating", "FinalRatingScore", "IsFinalized", "FinalizedAt" },
            data.Select(i => new[] { i.EvalId.ToString(), (i.EmployeeId ?? 0).ToString(), (i.CycleId ?? 0).ToString(), (i.SelfRating ?? 0).ToString(), (i.ManagerRating ?? 0).ToString(), (i.FinalRatingScore ?? 0).ToString(), i.IsFinalized == true ? "Yes" : "No", i.FinalizedAt?.ToString("yyyy-MM-dd") ?? "" }).ToList());
    }

    public async Task<byte[]> ExportPerformanceOutcomesToPdfAsync()
    {
        var data = (await _outcomeService.GetAllAsync()).ToList();
        return GeneratePdf("Performance Outcomes Report",
            new[] { "OutcomeId", "EvalId", "EmployeeId", "CycleId", "RecommendationType", "OldPositionId", "NewPositionId", "ApprovalStatus", "EffectiveDate" },
            data.Select(i => new[] { i.OutcomeId.ToString(), (i.EvalId ?? 0).ToString(), (i.EmployeeId ?? 0).ToString(), (i.CycleId ?? 0).ToString(), i.RecommendationType ?? "", (i.OldPositionId ?? 0).ToString(), (i.NewPositionId ?? 0).ToString(), i.ApprovalStatus ?? "", i.EffectiveDate?.ToString("yyyy-MM-dd") ?? "" }).ToList());
    }

    public async Task<byte[]> ExportDepartmentsToPdfAsync()
    {
        var data = (await _departmentService.GetAllAsync()).ToList();
        return GeneratePdf("Departments Report", new[] { "DepartmentId", "DepartmentName", "ParentDepartmentId", "IsActive" },
            data.Select(i => new[] { i.DepartmentId.ToString(), i.DepartmentName, (i.ParentDepartmentId ?? 0).ToString(), i.IsActive ? "Yes" : "No" }).ToList());
    }

    public async Task<byte[]> ExportTeamsToPdfAsync()
    {
        var data = (await _teamService.GetAllAsync()).ToList();
        return GeneratePdf("Teams Report", new[] { "TeamId", "TeamName", "ManagerId", "DepartmentId" },
            data.Select(i => new[] { i.TeamId.ToString(), i.TeamName, (i.ManagerId ?? 0).ToString(), (i.DepartmentId ?? 0).ToString() }).ToList());
    }

    public async Task<byte[]> ExportEmployeesToPdfAsync()
    {
        var data = (await _employeeService.GetAllAsync()).ToList();
        return GeneratePdf("Employees Report", 
            new[] { "Code", "Full Name", "Email", "Phone", "Dept", "Pos", "Join Date", "Status" },
            data.Select(i => new[] { 
                i.EmployeeCode, 
                i.FullName, 
                i.Email ?? "", 
                i.Phone ?? "", 
                i.DepartmentName ?? "", 
                i.PositionTitle ?? "", 
                i.JoinDate?.ToString("yyyy-MM-dd") ?? "", 
                (i.IsActive ?? true) ? "Active" : "Inactive" 
            }).ToList());
    }

    private static void StyleHeaderRow(IXLWorksheet ws, int columnCount)
    {
        var headerRange = ws.Range(1, 1, 1, columnCount);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }

    private static byte[] WorkbookToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static byte[] GeneratePdf(string title, string[] headers, List<string[]> rows)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(header =>
                {
                    header.Column(col =>
                    {
                        col.Item().Text("ACE Data Systems Ltd.").FontSize(10).SemiBold();
                        col.Item().Text(title).FontSize(14).Bold();
                        col.Item().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(8).Italic();
                        col.Item().PaddingBottom(10);
                    });
                });

                page.Content().Element(content =>
                {
                    content.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            for (int i = 0; i < headers.Length; i++)
                                columns.RelativeColumn();
                        });

                        foreach (var h in headers)
                        {
                            table.Cell().Background(Colors.Blue.Medium).Padding(4)
                                .Text(h).FontColor(Colors.White).Bold().FontSize(8);
                        }

                        bool alternate = false;
                        foreach (var dataRow in rows)
                        {
                            var bgColor = alternate ? Colors.Grey.Lighten4 : Colors.White;
                            foreach (var cell in dataRow)
                            {
                                table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3)
                                    .Text(cell).FontSize(8);
                            }
                            alternate = !alternate;
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Page ").FontSize(8);
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" of ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private static int? ParseNullableInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "0") return null;
        return int.TryParse(value, out var result) ? result : null;
    }

    private static decimal? ParseNullableDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "0") return null;
        return decimal.TryParse(value, out var result) ? result : null;
    }

    private static DateTime? ParseNullableDateTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return DateTime.TryParse(value, out var result) ? result : null;
    }

    private static DateOnly? ParseNullableDateOnly(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return DateOnly.TryParse(value, out var result) ? result : null;
    }
}
