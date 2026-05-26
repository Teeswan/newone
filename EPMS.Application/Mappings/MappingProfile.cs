using AutoMapper;
using EPMS.Domain.Entities;     
using EPMS.Shared.DTOs;          
using EPMS.Shared.Requests;      
using EPMS.Domain.Enums;
using EPMS.Application.UseCases.PositionKpi.Commands;

namespace EPMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Appraisal Form Mappings
        CreateMap<ApplicationForm, AppraisalFormDto>();
        CreateMap<CreateAppraisalFormRequest, ApplicationForm>();
        CreateMap<UpdateAppraisalFormRequest, ApplicationForm>();

        // Appraisal Question Mappings
        CreateMap<AppraisalQuestion, AppraisalQuestionDto>();
        CreateMap<CreateAppraisalQuestionRequest, AppraisalQuestion>();
        CreateMap<UpdateAppraisalQuestionRequest, AppraisalQuestion>();

        // Appraisal Response Mappings
        CreateMap<AppraisalResponse, AppraisalResponseDto>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question != null ? src.Question.QuestionText : null))
            .ForMember(dest => dest.QuestionCategory, opt => opt.MapFrom(src => src.Question != null ? src.Question.Category : null))
            .ForMember(dest => dest.RespondentName, opt => opt.MapFrom(src => src.Respondent != null ? src.Respondent.FullName : null))
            .ForMember(dest => dest.RespondentPosition, opt => opt.MapFrom(src => src.Respondent != null && src.Respondent.Position != null ? src.Respondent.Position.PositionTitle : null))
            .ForMember(dest => dest.RespondentDepartment, opt => opt.MapFrom(src => src.Respondent != null && src.Respondent.Department != null ? src.Respondent.Department.DepartmentName : null))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => (src.Question != null && src.Eval != null && src.Question.FormQuestions != null) 
                ? src.Question.FormQuestions.Where(fq => fq.FormId == src.Eval.FormId).Select(fq => (int?)fq.SortOrder).FirstOrDefault() ?? 0 : 0));
        CreateMap<CreateAppraisalResponseRequest, AppraisalResponse>();
        CreateMap<UpdateAppraisalResponseRequest, AppraisalResponse>();

        // Performance Evaluation Mappings
        CreateMap<PerformanceEvaluation, PerformanceEvaluationDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
            .ForMember(dest => dest.CycleName, opt => opt.MapFrom(src => src.Cycle != null ? src.Cycle.CycleName : null))
            .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form != null ? src.Form.FormName : null))
            .ForMember(dest => dest.Responses, opt => opt.MapFrom(src => src.AppraisalResponses));
        CreateMap<CreatePerformanceEvaluationRequest, PerformanceEvaluation>();
        CreateMap<UpdatePerformanceEvaluationRequest, PerformanceEvaluation>();

        // Performance Outcome Mappings
        CreateMap<PerformanceOutcome, PerformanceOutcomeDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
            .ForMember(dest => dest.CycleName, opt => opt.MapFrom(src => src.Cycle != null ? src.Cycle.CycleName : null));
        CreateMap<CreatePerformanceOutcomeRequest, PerformanceOutcome>();
        CreateMap<UpdatePerformanceOutcomeRequest, PerformanceOutcome>();

        CreateMap<AppraisalCycle, AppraisalCycleDto>();
        CreateMap<CreateAppraisalCycleRequest, AppraisalCycle>();
        CreateMap<UpdateAppraisalCycleRequest, AppraisalCycle>();

        CreateMap<FormQuestion, FormQuestionDto>();
        CreateMap<CreateFormQuestionRequest, FormQuestion>();
        CreateMap<UpdateFormQuestionRequest, FormQuestion>();

        CreateMap<MeetingNote, NoteDto>().ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Email : null))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null));

        // Org & Security Mappings
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.ParentDepartmentName, opt => opt.MapFrom(src => src.ParentDepartment != null ? src.ParentDepartment.DepartmentName : null));
        CreateMap<CreateDepartmentRequest, Department>();
        CreateMap<UpdateDepartmentRequest, Department>();
        CreateMap<Department, DepartmentTreeDto>()
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.InverseParentDepartment));

        CreateMap<Team, TeamDto>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null));
        CreateMap<CreateTeamRequest, Team>();
        CreateMap<UpdateTeamRequest, Team>();

        CreateMap<Permission, PermissionDto>();
        CreateMap<PositionPermission, PositionPermissionDto>()
            .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position != null ? src.Position.PositionTitle : null))
            .ForMember(dest => dest.PermissionCode, opt => opt.MapFrom(src => src.Permission != null ? src.Permission.PermissionCode : null));

        // Employee & Personnel Mappings
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.PositionId))
            .ForMember(dest => dest.ReportsTo, opt => opt.MapFrom(src => src.ReportsTo))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null))
            .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position != null ? src.Position.PositionTitle : null))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.ReportsToNavigation != null ? src.ReportsToNavigation.FullName : null))
            .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamsNavigation != null && src.TeamsNavigation.Any() ? (int?)src.TeamsNavigation.First().TeamId : null))
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.TeamsNavigation != null && src.TeamsNavigation.Any() ? src.TeamsNavigation.First().TeamName : null));
        
        CreateMap<Employee, EmployeeDetailDto>()
            .IncludeBase<Employee, EmployeeDto>();
        
        CreateMap<CreateEmployeeRequest, Employee>();
        CreateMap<UpdateEmployeeRequest, Employee>();

        CreateMap<Level, LevelDto>();
        CreateMap<CreateLevelRequest, Level>();
        CreateMap<UpdateLevelRequest, Level>();

        CreateMap<Position, PositionDto>()
            .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.Level != null ? src.Level.LevelName : null));
        CreateMap<CreatePositionRequest, Position>();
        CreateMap<UpdatePositionRequest, Position>();

        // KPI Hierarchy Mappings
        CreateMap<Kpi, KpiDto>();
        CreateMap<DepartmentKpi, DepartmentKpiDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null))
            .ForMember(dest => dest.CycleName, opt => opt.MapFrom(src => src.Cycle != null ? src.Cycle.CycleName : null))
            .ForMember(dest => dest.KpiName, opt => opt.MapFrom(src => src.Kpi != null ? src.Kpi.KpiName : null));
        CreateMap<TeamKpi, TeamKpiDto>()
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.TeamName : null))
            .ForMember(dest => dest.KpiName, opt => opt.MapFrom(src => src.DepartmentKpi != null && src.DepartmentKpi.Kpi != null ? src.DepartmentKpi.Kpi.KpiName : null))
            .ForMember(dest => dest.ParentTarget, opt => opt.MapFrom(src => src.DepartmentKpi != null ? src.DepartmentKpi.DepartmentTarget : 0));
        CreateMap<EmployeeKpi, EmployeeKpiDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
            .ForMember(dest => dest.KpiName, opt => opt.MapFrom(src => src.TeamKpi != null && src.TeamKpi.DepartmentKpi != null && src.TeamKpi.DepartmentKpi.Kpi != null ? src.TeamKpi.DepartmentKpi.Kpi.KpiName : null))
            .ForMember(dest => dest.ParentTarget, opt => opt.MapFrom(src => src.TeamKpi != null ? src.TeamKpi.TeamTarget : 0));

        CreateMap<OneOnOneMeeting, MeetingDto>()
            .ForMember(dest => dest.MeetingId, opt => opt.MapFrom(src => src.MeetingId))
            .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.ManagerId))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
            .ForMember(dest => dest.ScheduledDateTime, opt => opt.MapFrom(src => src.ScheduledDateTime))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.MeetingStatus, opt => opt.MapFrom(src => src.MeetingStatus))
            .ForMember(dest => dest.RescheduleReason, opt => opt.MapFrom(src => src.RescheduleReason))
            .ForMember(dest => dest.ParentMeetingId, opt => opt.MapFrom(src => src.ParentMeetingId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.DiscussionPoints, opt => opt.MapFrom(src => src.DiscussionPoints))
            .ForMember(dest => dest.ActionItems, opt => opt.MapFrom(src => src.ActionItems))
            .ForMember(dest => dest.MeetingSummary, opt => opt.MapFrom(src => src.MeetingSummary))
            .ForMember(dest => dest.CanJoin, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var buffer = context.Items.ContainsKey("JoinBufferMinutes") ? (int)context.Items["JoinBufferMinutes"] : 15;
                var duration = context.Items.ContainsKey("StandardDuration") ? (int)context.Items["StandardDuration"] : 45;
                var now = DateTime.UtcNow;
                
                if (!src.ScheduledDateTime.HasValue)
                    return false;
                    
                var endTime = src.ScheduledDateTime.Value.AddMinutes(duration);
          
                bool isWithinBuffer = now >= src.ScheduledDateTime.Value.AddMinutes(-buffer) && now <= endTime;
                bool isManagerAlreadyIn = src.MeetingStatus == "InProgress";

                return isWithinBuffer || isManagerAlreadyIn;
            }))
            .ForMember(dest => dest.Notes, opt => opt.Ignore());

        CreateMap<MeetingNote, MeetingNoteDto>()
            .ForMember(dest => dest.NoteId, opt => opt.MapFrom(src => src.NoteId))
            .ForMember(dest => dest.MeetingId, opt => opt.MapFrom(src => src.MeetingId))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.FullName : null))
            .ForMember(dest => dest.NoteContent, opt => opt.MapFrom(src => src.NoteText))
            .ForMember(dest => dest.NoteType, opt => opt.MapFrom(src => src.NoteType))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Entity ? DTO
        CreateMap<PipPlan, PipPlanDto>()
            .ForMember(d => d.PipId, o => o.MapFrom(s => s.Pipid))
            .ForMember(d => d.EmployeeName,
                o => o.MapFrom(s => s.Employee != null
                    ? s.Employee.FullName : string.Empty))
            .ForMember(d => d.ManagerName,
                o => o.MapFrom(s => s.Manager != null
                    ? s.Manager.FullName : string.Empty))
            .ForMember(d => d.CreatedAt,
                o => o.MapFrom(s => s.CreatedAt ?? DateTime.MinValue))
            .ForMember(d => d.Objectives, o => o.MapFrom(s => s.PipObjectives))
            .ForMember(d => d.Meetings, o => o.MapFrom(s => s.PipMeetings));

        CreateMap<PipObjective, PipObjectiveDto>()
            .ForMember(d => d.PipId, o => o.MapFrom(s => s.Pipid ?? 0))
            .ForMember(d => d.IsAchieved, o => o.MapFrom(s => s.IsAchieved ?? false));

        CreateMap<PipMeeting, PipMeetingDto>()
            .ForMember(d => d.PipId, o => o.MapFrom(s => s.Pipid ?? 0));

        // Request ? Entity
        CreateMap<CreatePipPlanRequest, PipPlan>()
            .ForMember(d => d.Pipid, o => o.Ignore())
            .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
            .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
            .ForMember(d => d.Status, o => o.MapFrom(_ => "Active"))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.PipObjectives, o => o.Ignore())
            .ForMember(d => d.PipMeetings, o => o.Ignore())
            .ForMember(d => d.Employee, o => o.Ignore())
            .ForMember(d => d.Manager, o => o.Ignore());

        CreateMap<CreatePipObjectiveRequest, PipObjective>()
            .ForMember(d => d.ObjectiveId, o => o.Ignore())
            .ForMember(d => d.Pipid, o => o.MapFrom(s => s.PipId))
            .ForMember(d => d.IsAchieved, o => o.MapFrom(_ => false))
            //.ForMember(d => d.ReviewComments, o => o.Ignore())
            .ForMember(d => d.Pip, o => o.Ignore());

        CreateMap<CreatePipMeetingRequest, PipMeeting>()
            .ForMember(d => d.PipMeetingId, o => o.Ignore())
            .ForMember(d => d.Pipid, o => o.MapFrom(s => s.PipId))
            .ForMember(d => d.Pip, o => o.Ignore());

        CreateMap<PositionKpi, PositionKpiDto>()
   .ForMember(dest => dest.PositionKpiId,
       opt => opt.MapFrom(src => src.PositionKpiId))

   .ForMember(dest => dest.KpiId,
       opt => opt.MapFrom(src => src.KpiId))

   .ForMember(dest => dest.KpiName,
       opt => opt.MapFrom(src => src.Kpi.KpiName))

   .ForMember(dest => dest.Category,
       opt => opt.MapFrom(src => src.Kpi.Category))

   .ForMember(dest => dest.Unit,
       opt => opt.MapFrom(src => src.Kpi.Unit))

   .ForMember(dest => dest.WeightPercent,
       opt => opt.MapFrom(src => src.DefaultWeightPercent))

   .ForMember(dest => dest.Direction,
       opt => opt.MapFrom(src => src.Kpi.Direction))

   .ForMember(dest => dest.PositionId,
       opt => opt.MapFrom(src => src.PositionId))

   .ForMember(dest => dest.PositionName,
       opt => opt.MapFrom(src =>
           src.Position != null
               ? src.Position.PositionTitle
               : null))

   .ForMember(dest => dest.IsRequired,
       opt => opt.MapFrom(src => src.IsRequired))

   .ForMember(dest => dest.IsKpiActive,
       opt => opt.MapFrom(src => src.Kpi.IsActive))

   .ForMember(dest => dest.IsPositionKpiActive,
       opt => opt.MapFrom(src => src.IsActive));

        //CreateMap<CreatePositionKpiRequest, PositionKpi>();
        //CreateMap<UpdatePositionKpiRequest, PositionKpi>();

        // Import Mapping (for the Excel Bulk Import feature)
        CreateMap<PositionKpiImportDto, PositionKpi>();
    }
}
