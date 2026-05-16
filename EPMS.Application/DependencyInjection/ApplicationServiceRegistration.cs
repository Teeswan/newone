using EPMS.Application.Interfaces;
using EPMS.Application.Mappings;
using EPMS.Application.Services;
using EPMS.Application.Settings;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Services;
using EPMS.Shared.Requests;
using EPMS.Shared.Validation;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPMS.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MeetingSettings>(configuration.GetSection("MeetingSettings"));

        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        services.AddScoped<IAppraisalCycleService, AppraisalCycleService>();
        services.AddScoped<IAppraisalQuestionService, AppraisalQuestionService>();
        services.AddScoped<IAppraisalResponseService, AppraisalResponseService>();
        services.AddScoped<IAppraisalFormService, AppraisalFormService>();
        services.AddScoped<IFormQuestionService, FormQuestionService>();
        services.AddScoped<IPerformanceEvaluationService, PerformanceEvaluationService>();
        services.AddScoped<IPerformanceOutcomeService, PerformanceOutcomeService>();
        services.AddScoped<IExcelPdfService, ExcelPdfService>();

        services.AddScoped<IMeetingService, MeetingService>();

        // Org & Security Services
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IPermissionService, PermissionService>();

        // Employee & Personnel Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ILevelService, LevelService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<INotificationService, NotificationService>();

        // Domain Services
        services.AddScoped<IKpiScoreCalculator, KpiScoreCalculator>();
        services.AddScoped<KpiWeightValidator>();

        // MediatR & FluentValidation
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly));
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);

        // FluentValidation (server-side gate)
        services.AddScoped<IValidator<CreatePipPlanRequest>, CreatePipPlanValidator>();
        services.AddScoped<IValidator<CreatePipObjectiveRequest>, CreatePipObjectiveValidator>();
        services.AddScoped<IValidator<CreatePipMeetingRequest>, CreatePipMeetingValidator>();
        services.AddScoped<IValidator<UpdatePipObjectiveRequest>, UpdatePipObjectiveValidator>();

        // Services
        services.AddScoped<IPipService, PipService>();

        return services;
    }
}
