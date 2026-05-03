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
                return src.ScheduledDateTime.HasValue ? src.ScheduledDateTime.Value.AddMinutes(duration) : (DateTime?)null;
            }))
            .ForMember(dest => dest.CanJoin, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var buffer = context.Items.ContainsKey("JoinBufferMinutes") ? (int)context.Items["JoinBufferMinutes"] : 0;
                var duration = context.Items.ContainsKey("StandardDuration") ? (int)context.Items["StandardDuration"] : 45;
                var now = DateTime.UtcNow;
                
                if (!src.ScheduledDateTime.HasValue)
                    return false;
                    
                var endTime = src.ScheduledDateTime.Value.AddMinutes(duration);
          
                bool isWithinBuffer = now >= src.ScheduledDateTime.Value.AddMinutes(-buffer) && now <= endTime;
                bool isManagerAlreadyIn = src.MeetingStatus == "Started";

                return isWithinBuffer || isManagerAlreadyIn;
            }));

        // Add more mappings as needed for other entities
    }
}
