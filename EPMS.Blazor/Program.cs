using Blazored.LocalStorage;
using EPMS.Blazor;
using EPMS.Blazor.Services;
using EPMS.Client.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register LocalStorage and Auth State
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

// Register the Authentication Handler
builder.Services.AddScoped<AuthenticationHeaderHandler>();

// API Base URL
var apiBaseUrl = new Uri("https://localhost:7202/");

// Register Typed Clients with the Authentication Handler
builder.Services.AddHttpClient<IEmployeeBlazorService, EmployeeBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IDepartmentBlazorService, DepartmentBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<ITeamBlazorService, TeamBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IPipClientService, PipClientService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<ILevelBlazorService, LevelBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IPositionBlazorService, PositionBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IPermissionBlazorService, PermissionBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IUserBlazorService, UserBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IAppraisalCycleBlazorService, AppraisalCycleBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IAppraisalFormBlazorService, AppraisalFormBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IAppraisalQuestionBlazorService, AppraisalQuestionBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IAppraisalResponseBlazorService, AppraisalResponseBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IPerformanceEvaluationBlazorService, PerformanceEvaluationBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IPerformanceOutcomeBlazorService, PerformanceOutcomeBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IFormQuestionBlazorService, FormQuestionBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IPositionKpiBlazorService, PositionKpiBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IEmployeeKpiBlazorService, EmployeeKpiBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient<IMeetingBlazorService, MeetingBlazorService>(client => client.BaseAddress = apiBaseUrl)
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

// AuthBlazorService should NOT use AuthenticationHeaderHandler (no token needed for login/change password)
builder.Services.AddHttpClient<IAuthBlazorService, AuthBlazorService>(client => client.BaseAddress = apiBaseUrl);

// Register a default HttpClient for components that inject it directly (like Login.razor)
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Default"));
builder.Services.AddHttpClient("Default", client => client.BaseAddress = apiBaseUrl);

// Register MudServices
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
});

await builder.Build().RunAsync();
