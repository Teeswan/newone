using Blazored.LocalStorage;
using EPMS.Blazor;
using EPMS.Blazor.Services;
using EPMS.Client.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5111/")
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

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

builder.Services.AddScoped<IAppraisalCycleBlazorService, AppraisalCycleBlazorService>();
builder.Services.AddScoped<IAppraisalFormBlazorService, AppraisalFormBlazorService>();
builder.Services.AddScoped<IAppraisalQuestionBlazorService, AppraisalQuestionBlazorService>();
builder.Services.AddScoped<IAppraisalResponseBlazorService, AppraisalResponseBlazorService>();
builder.Services.AddScoped<IPerformanceEvaluationBlazorService, PerformanceEvaluationBlazorService>();
builder.Services.AddScoped<IPerformanceOutcomeBlazorService, PerformanceOutcomeBlazorService>();
builder.Services.AddScoped<IFormQuestionBlazorService, FormQuestionBlazorService>();
builder.Services.AddScoped<IDepartmentBlazorService, DepartmentBlazorService>();
builder.Services.AddScoped<ITeamBlazorService, TeamBlazorService>();
builder.Services.AddScoped<IPipClientService, PipClientService>();
builder.Services.AddScoped<IKpiMasterBlazorService, KpiMasterBlazorService>();
builder.Services.AddScoped<IEmployeeKpiBlazorService, EmployeeKpiBlazorService>();

await builder.Build().RunAsync();