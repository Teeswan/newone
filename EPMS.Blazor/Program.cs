using Blazored.LocalStorage;
using EPMS.Blazor;
//using EPMS.Blazor.Identity;
using EPMS.Blazor.Services;
using EPMS.Client.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
// --------------------------------------------------

builder.Services.AddScoped<IAppraisalCycleBlazorService, AppraisalCycleBlazorService>();
builder.Services.AddScoped<IAppraisalFormBlazorService, AppraisalFormBlazorService>();
builder.Services.AddScoped<IAppraisalQuestionBlazorService, AppraisalQuestionBlazorService>();
builder.Services.AddScoped<IAppraisalResponseBlazorService, AppraisalResponseBlazorService>();
builder.Services.AddScoped<IPerformanceEvaluationBlazorService, PerformanceEvaluationBlazorService>();
builder.Services.AddScoped<IPerformanceOutcomeBlazorService, PerformanceOutcomeBlazorService>();
builder.Services.AddScoped<IFormQuestionBlazorService, FormQuestionBlazorService>();

await builder.Build().RunAsync();