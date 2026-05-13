using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EPMS.Application.UseCases.KpiMaster.Commands;
using EPMS.Application.UseCases.KpiAssignment.Commands;

namespace EPMS.Application.Interfaces;

public interface IExcelPdfService
{
    // Excel Export
    Task<byte[]> ExportAppraisalCyclesToExcelAsync();
    Task<byte[]> ExportAppraisalQuestionsToExcelAsync();
    Task<byte[]> ExportAppraisalResponsesToExcelAsync();
    Task<byte[]> ExportAppraisalFormsToExcelAsync();
    Task<byte[]> ExportFormQuestionsToExcelAsync();
    Task<byte[]> ExportPerformanceEvaluationsToExcelAsync();
    Task<byte[]> ExportPerformanceOutcomesToExcelAsync();
    Task<byte[]> ExportDepartmentsToExcelAsync();
    Task<byte[]> ExportTeamsToExcelAsync();

    // Excel Import
    Task<int> ImportAppraisalCyclesFromExcelAsync(Stream fileStream);
    Task<int> ImportAppraisalQuestionsFromExcelAsync(Stream fileStream);
    Task<int> ImportAppraisalResponsesFromExcelAsync(Stream fileStream);
    Task<int> ImportAppraisalFormsFromExcelAsync(Stream fileStream);
    Task<int> ImportFormQuestionsFromExcelAsync(Stream fileStream);
    Task<int> ImportPerformanceEvaluationsFromExcelAsync(Stream fileStream);
    Task<int> ImportPerformanceOutcomesFromExcelAsync(Stream fileStream);
    Task<int> ImportDepartmentsFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false);
    Task<int> ImportTeamsFromExcelAsync(Stream fileStream, bool skipFirstRow = true, string sheetName = "", bool skipExisting = false);
    Task<IEnumerable<KpiImportDto>> ImportKpiMasterFromExcelAsync(Stream fileStream);
    Task<byte[]> ExportKpiMasterTemplateAsync();
    Task<IEnumerable<EmployeeKpiImportDto>> ImportEmployeeKpiFromExcelAsync(Stream fileStream);
    Task<byte[]> ExportEmployeeKpiTemplateAsync();

    // PDF Export
    Task<byte[]> ExportAppraisalCyclesToPdfAsync();
    Task<byte[]> ExportAppraisalQuestionsToPdfAsync();
    Task<byte[]> ExportAppraisalResponsesToPdfAsync();
    Task<byte[]> ExportAppraisalFormsToPdfAsync();
    Task<byte[]> ExportFormQuestionsToPdfAsync();
    Task<byte[]> ExportPerformanceEvaluationsToPdfAsync();
    Task<byte[]> ExportPerformanceOutcomesToPdfAsync();
    Task<byte[]> ExportDepartmentsToPdfAsync();
    Task<byte[]> ExportTeamsToPdfAsync();
}
