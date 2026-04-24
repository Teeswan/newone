using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using EPMS.Infrastructure.Repositories;
using EPMS.Infrastructure.Authorization;
using EPMS.Infrastructure.Cache;
using EPMS.Infrastructure.DataAccess;
using EPMS.Infrastructure.Services;
using EPMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;

namespace EPMS.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        var defaultCacheDurationMinutes = configuration.GetValue<int?>("CacheSettings:DefaultCacheDurationMinutes") ?? 10;
        var defaultCacheDuration = TimeSpan.FromMinutes(defaultCacheDurationMinutes);

        // Appraisal Cycle Repository with Caching
        services.AddScoped<IAppraisalCycleRepository, AppraisalCycleRepository>();
        services.Decorate<IAppraisalCycleRepository>((inner, provider) =>
            new CachedAppraisalCycleRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // Appraisal Response Repository with Caching
        services.AddScoped<IAppraisalResponseRepository, AppraisalResponseRepository>();
        services.Decorate<IAppraisalResponseRepository>((inner, provider) =>
            new CachedAppraisalResponseRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // Appraisal Question Repository with Caching
        services.AddScoped<IAppraisalQuestionRepository, AppraisalQuestionRepository>();
        services.Decorate<IAppraisalQuestionRepository>((inner, provider) =>
            new CachedAppraisalQuestionRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // Appraisal Form Repository with Caching
        services.AddScoped<IAppraisalFormRepository, ApplicationFormRepository>();
        services.Decorate<IAppraisalFormRepository>((inner, provider) =>
            new CachedAppraisalFormRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IFormQuestionRepository>(provider =>
            new FormQuestionRepository(
                provider.GetRequiredService<AppDbContext>(),
                provider.GetRequiredService<ISqlRepository<FormQuestion, object[]>>()));

        services.AddScoped(typeof(ISqlRepository<,>), typeof(SqlRepository<,>));



        // Performance Evaluation Repository with Caching
        services.AddScoped<IPerformanceEvaluationRepository, PerformanceEvaluationRepository>();
        services.Decorate<IPerformanceEvaluationRepository>((inner, provider) =>
            new CachedPerformanceEvaluationRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // Performance Outcome Repository with Caching
        services.AddScoped<IPerformanceOutcomeRepository, PerformanceOutcomeRepository>();
        services.Decorate<IPerformanceOutcomeRepository>((inner, provider) =>
            new CachedPerformanceOutcomeRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // Meeting and Unit of Work
        services.AddScoped<IMeetingRepository, MeetingRepository>();
        services.AddScoped<IMeetingNoteRepository, MeetingNoteRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Org & Security Repositories with Caching
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.Decorate<IDepartmentRepository>((inner, provider) =>
            new CachedDepartmentRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<ITeamRepository, TeamRepository>();
        services.Decorate<ITeamRepository>((inner, provider) =>
            new CachedTeamRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IPositionPermissionRepository, PositionPermissionRepository>();
        services.Decorate<IPositionPermissionRepository>((inner, provider) =>
            new CachedPositionPermissionRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // Employee & Personnel Repositories with Caching
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.Decorate<IEmployeeRepository>((inner, provider) =>
            new CachedEmployeeRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<ILevelRepository, LevelRepository>();
        services.Decorate<ILevelRepository>((inner, provider) =>
            new CachedLevelRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        services.AddScoped<IPositionRepository, PositionRepository>();
        services.Decorate<IPositionRepository>((inner, provider) =>
            new CachedPositionRepository(inner, provider.GetRequiredService<IMemoryCache>(), defaultCacheDuration));

        // Generic Permission Repository
        services.AddScoped<IBaseRepository<Permission, int>, BaseRepository<Permission, int>>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Authorization
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        // KPI Module
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IKpiMasterRepository, KpiMasterRepository>();
        services.AddScoped<IKpiAssignmentRepository, KpiAssignmentRepository>();
        services.AddScoped<IKpiCacheService, KpiCacheService>();
        services.AddScoped<IKpiQueryService, KpiQueryService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        return services;
    }
}
