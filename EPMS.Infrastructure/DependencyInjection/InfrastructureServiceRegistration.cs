using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Authorization;
using EPMS.Infrastructure.Cache;
using EPMS.Infrastructure.Contexts;
using EPMS.Infrastructure.DataAccess;
using EPMS.Infrastructure.Interceptors;
using EPMS.Infrastructure.Repositories;
using EPMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPMS.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ?? Memory Cache ??????????????????????????????????????????????????????
        services.AddMemoryCache();

        var defaultCacheDurationMinutes =
            configuration.GetValue<int?>("CacheSettings:DefaultCacheDurationMinutes") ?? 10;
        var defaultCacheDuration = TimeSpan.FromMinutes(defaultCacheDurationMinutes);

        // ?? EF Core Audit Interceptor ?????????????????????????????????????????
        services.AddSingleton<AuditSaveChangesInterceptor>();

        // ?? EF Core DbContext (single registration with interceptor) ??????????
        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.CommandTimeout(30));

            opt.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // ?? Appraisal Repositories ????????????????????????????????????????????
        services.AddScoped<IAppraisalCycleRepository, AppraisalCycleRepository>();
        services.Decorate<IAppraisalCycleRepository>((inner, provider) =>
            new CachedAppraisalCycleRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IAppraisalResponseRepository, AppraisalResponseRepository>();
        services.Decorate<IAppraisalResponseRepository>((inner, provider) =>
            new CachedAppraisalResponseRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IAppraisalQuestionRepository, AppraisalQuestionRepository>();
        services.Decorate<IAppraisalQuestionRepository>((inner, provider) =>
            new CachedAppraisalQuestionRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IAppraisalFormRepository, ApplicationFormRepository>();
        services.Decorate<IAppraisalFormRepository>((inner, provider) =>
            new CachedAppraisalFormRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // ?? Form & SQL Repositories ???????????????????????????????????????????
        services.AddScoped(typeof(ISqlRepository<,>), typeof(SqlRepository<,>));

        services.AddScoped<IFormQuestionRepository>(provider =>
            new FormQuestionRepository(
                provider.GetRequiredService<AppDbContext>(),
                provider.GetRequiredService<ISqlRepository<FormQuestion, object[]>>()));

        // ?? Performance Repositories ??????????????????????????????????????????
        services.AddScoped<IPerformanceEvaluationRepository, PerformanceEvaluationRepository>();
        services.Decorate<IPerformanceEvaluationRepository>((inner, provider) =>
            new CachedPerformanceEvaluationRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IPerformanceOutcomeRepository, PerformanceOutcomeRepository>();
        services.Decorate<IPerformanceOutcomeRepository>((inner, provider) =>
            new CachedPerformanceOutcomeRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // ?? Meeting Repositories ??????????????????????????????????????????????
        services.AddScoped<IMeetingRepository, MeetingRepository>();
        services.AddScoped<IMeetingNoteRepository, MeetingNoteRepository>();

        // ?? User & Role Repositories ??????????????????????????????????????????
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // ?? Unit of Work ??????????????????????????????????????????????????????
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ?? Org Repositories ??????????????????????????????????????????????????
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.Decorate<IDepartmentRepository>((inner, provider) =>
            new CachedDepartmentRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<ITeamRepository, TeamRepository>();
        services.Decorate<ITeamRepository>((inner, provider) =>
            new CachedTeamRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IPositionPermissionRepository, PositionPermissionRepository>();
        services.Decorate<IPositionPermissionRepository>((inner, provider) =>
            new CachedPositionPermissionRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // ?? Employee & Personnel Repositories ?????????????????????????????????
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.Decorate<IEmployeeRepository>((inner, provider) =>
            new CachedEmployeeRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<ILevelRepository, LevelRepository>();
        services.Decorate<ILevelRepository>((inner, provider) =>
            new CachedLevelRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IPositionRepository, PositionRepository>();
        services.Decorate<IPositionRepository>((inner, provider) =>
            new CachedPositionRepository(
                inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // ?? Authorization ?????????????????????????????????????????????????????
        services.AddScoped<IBaseRepository<Permission, int>, BaseRepository<Permission, int>>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        // ?? KPI Module ????????????????????????????????????????????????????????
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IKpiMasterRepository, KpiMasterRepository>();
        services.AddScoped<IKpiAssignmentRepository, KpiAssignmentRepository>();
        services.AddScoped<IKpiCacheService, KpiCacheService>();
        services.AddScoped<IKpiQueryService, KpiQueryService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        // ?? PIP Module (EF Core CRUD + Dapper Reports) ????????????????????????
        services.AddScoped<IPipPlanRepository, PipPlanRepository>();
        services.AddScoped<IPipObjectiveRepository, PipObjectiveRepository>();
        services.AddScoped<IPipMeetingRepository, PipMeetingRepository>();
        services.AddScoped<IPipReportRepository, PipReportRepository>();

        return services;
    }
}