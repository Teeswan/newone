namespace EPMS.Infrastructure;

public static class StoredProcedures
{
    // Appraisal Cycle
    public const string AppraisalCycles_GetAll = "sp_AppraisalCycles_GetAll";
    public const string AppraisalCycles_GetById = "sp_AppraisalCycles_GetById";
    public const string AppraisalCycles_Create = "sp_AppraisalCycles_Create";
    public const string AppraisalCycles_Update = "sp_AppraisalCycles_Update";
    public const string AppraisalCycles_Delete = "sp_AppraisalCycles_Delete";

    // Appraisal Response
    public const string AppraisalResponses_GetAll = "sp_AppraisalResponses_GetAll";
    public const string AppraisalResponses_GetById = "sp_AppraisalResponses_GetById";
    public const string AppraisalResponses_Create = "sp_AppraisalResponses_Create";
    public const string AppraisalResponses_Update = "sp_AppraisalResponses_Update";
    public const string AppraisalResponses_Delete = "sp_AppraisalResponses_Delete";
    public const string AppraisalResponses_GetByEvalId = "sp_AppraisalResponses_GetByEvalId";

    // Appraisal Question
    public const string AppraisalQuestions_GetAll = "sp_AppraisalQuestions_GetAll";
    public const string AppraisalQuestions_GetById = "sp_AppraisalQuestions_GetById";
    public const string AppraisalQuestions_Create = "sp_AppraisalQuestions_Create";
    public const string AppraisalQuestions_Update = "sp_AppraisalQuestions_Update";
    public const string AppraisalQuestions_Delete = "sp_AppraisalQuestions_Delete";

    // Appraisal Form (ApplicationForm)
    public const string ApplicationForms_GetAll = "sp_ApplicationForms_GetAll";
    public const string ApplicationForms_GetById = "sp_ApplicationForms_GetById";
    public const string ApplicationForms_Create = "sp_ApplicationForms_Create";
    public const string ApplicationForms_Update = "sp_ApplicationForms_Update";
    public const string ApplicationForms_Delete = "sp_ApplicationForms_Delete";

    // Form Question
    public const string FormQuestions_GetAll = "sp_FormQuestions_GetAll";
    public const string FormQuestions_GetById = "sp_FormQuestions_GetById";
    public const string FormQuestions_Create = "sp_FormQuestions_Create";
    public const string FormQuestions_Update = "sp_FormQuestions_Update";
    public const string FormQuestions_Delete = "sp_FormQuestions_Delete";
    public const string FormQuestions_GetByFormId = "sp_FormQuestions_GetByFormId";
    public const string FormQuestions_GetByFormAndQuestionId = "sp_FormQuestions_GetByFormAndQuestionId";

    // Performance Evaluation
    public const string PerformanceEvaluations_GetAll = "sp_PerformanceEvaluations_GetAll";
    public const string PerformanceEvaluations_GetById = "sp_PerformanceEvaluations_GetById";
    public const string PerformanceEvaluations_Create = "sp_PerformanceEvaluations_Create";
    public const string PerformanceEvaluations_Update = "sp_PerformanceEvaluations_Update";
    public const string PerformanceEvaluations_Delete = "sp_PerformanceEvaluations_Delete";
    public const string PerformanceEvaluations_GetByEmployeeId = "sp_PerformanceEvaluations_GetByEmployeeId";
    public const string PerformanceEvaluations_GetByCycleId = "sp_PerformanceEvaluations_GetByCycleId";

    // Performance Outcome
    public const string PerformanceOutcomes_GetAll = "sp_PerformanceOutcomes_GetAll";
    public const string PerformanceOutcomes_GetById = "sp_PerformanceOutcomes_GetById";
    public const string PerformanceOutcomes_Create = "sp_PerformanceOutcomes_Create";
    public const string PerformanceOutcomes_Update = "sp_PerformanceOutcomes_Update";
    public const string PerformanceOutcomes_Delete = "sp_PerformanceOutcomes_Delete";
    public const string PerformanceOutcomes_GetByEmployeeId = "sp_PerformanceOutcomes_GetByEmployeeId";
    public const string PerformanceOutcomes_GetByCycleId = "sp_PerformanceOutcomes_GetByCycleId";

    // Org & Security
    public const string Departments_GetTree = "sp_GetDepartmentTree";
    public const string Permissions_GetByPosition = "sp_GetPermissionsByPosition";
    public const string Teams_GetByDepartment = "sp_GetTeamsByDepartment";

    // Employee & Personnel
    public const string Employees_GetDetails = "sp_GetEmployeeDetails";
    public const string Employees_GetHierarchy = "sp_GetOrganizationHierarchy";
}