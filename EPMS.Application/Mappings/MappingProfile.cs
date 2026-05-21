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
        CreateMap<AppraisalResponse, AppraisalResponseDto>();
        CreateMap<CreateAppraisalResponseRequest, AppraisalResponse>();
        CreateMap<UpdateAppraisalResponseRequest, AppraisalResponse>();

        // Performance Evaluation Mappings
        CreateMap<PerformanceEvaluation, PerformanceEvaluationDto>();
        CreateMap<CreatePerformanceEvaluationRequest, PerformanceEvaluation>();
        CreateMap<UpdatePerformanceEvaluationRequest, PerformanceEvaluation>();

        // Performance Outcome Mappings
        CreateMap<PerformanceOutcome, PerformanceOutcomeDto>();
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
        CreateMap<User, UserDto>().ReverseMap();

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

        // Inside your MappingProfile constructor:

        CreateMap<PositionKpi, PositionKpiDto>()
            .ForMember(dest => dest.KpiId, opt => opt.MapFrom(src => src.KpiId))
            .ForMember(dest => dest.PriorityLevel, opt => opt.MapFrom(src => src.PriorityLevel.ToString()))
            .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString()))
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.Position != null ? src.Position.PositionTitle : "N/A"));

        //CreateMap<CreatePositionKpiRequest, PositionKpi>();
        //CreateMap<UpdatePositionKpiRequest, PositionKpi>();

        // Import Mapping (for the Excel Bulk Import feature)
        CreateMap<PositionKpiImportDto, PositionKpi>();
    }
}
