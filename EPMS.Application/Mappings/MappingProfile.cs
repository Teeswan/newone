using AutoMapper;
using EPMS.Domain.Entities;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AppraisalCycle, AppraisalCycleDto>();
        CreateMap<CreateAppraisalCycleRequest, AppraisalCycle>();
        CreateMap<UpdateAppraisalCycleRequest, AppraisalCycle>();

        CreateMap<FormQuestion, FormQuestionDto>();
        CreateMap<CreateFormQuestionRequest, FormQuestion>();
        CreateMap<UpdateFormQuestionRequest, FormQuestion>();

        CreateMap<MeetingNote, NoteDto>().ReverseMap();

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
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null))
            .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position != null ? src.Position.PositionTitle : null))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.ReportsToNavigation != null ? src.ReportsToNavigation.FullName : null));
        
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
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.ScheduledDateTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.MeetingStatus))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var duration = context.Items.ContainsKey("StandardDuration") ? (int)context.Items["StandardDuration"] : 45;
                return src.ScheduledDateTime.AddMinutes(duration);
            }))
            .ForMember(dest => dest.CanJoin, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var buffer = context.Items.ContainsKey("JoinBufferMinutes") ? (int)context.Items["JoinBufferMinutes"] : 0;
                var duration = context.Items.ContainsKey("StandardDuration") ? (int)context.Items["StandardDuration"] : 45;
                var now = DateTime.UtcNow;
                var endTime = src.ScheduledDateTime.AddMinutes(duration);
          
                bool isWithinBuffer = now >= src.ScheduledDateTime.AddMinutes(-buffer) && now <= endTime;

                bool isManagerAlreadyIn = src.MeetingStatus == "Started";

                return isWithinBuffer || isManagerAlreadyIn;
            }));

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
                o => o.MapFrom(s => s.CreatedAt ?? DateTime.MinValue));

        CreateMap<PipObjective, PipObjectiveDto>()
            .ForMember(d => d.PipId, o => o.MapFrom(s => s.Pipid ?? 0))
            .ForMember(d => d.IsAchieved, o => o.MapFrom(s => s.IsAchieved ?? false));

        CreateMap<PipMeeting, PipMeetingDto>()
            .ForMember(d => d.PipId, o => o.MapFrom(s => s.Pipid ?? 0));

        // Request ? Entity
        CreateMap<CreatePipPlanRequest, PipPlan>()
            .ForMember(d => d.Pipid, o => o.Ignore())
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
            .ForMember(d => d.ReviewComments, o => o.Ignore())
            .ForMember(d => d.Pip, o => o.Ignore());

        CreateMap<CreatePipMeetingRequest, PipMeeting>()
            .ForMember(d => d.PipMeetingId, o => o.Ignore())
            .ForMember(d => d.Pipid, o => o.MapFrom(s => s.PipId))
            .ForMember(d => d.Pip, o => o.Ignore());

        // Add more mappings as needed for other entities
    }
}
