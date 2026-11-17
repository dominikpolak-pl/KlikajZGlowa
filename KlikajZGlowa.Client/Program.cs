using KlikajZGlowa.Client;
using KlikajZGlowa.Client.Interfaces;
using KlikajZGlowa.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient("KlikajZGlowaAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7173/"); 
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("KlikajZGlowaAPI"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<ICourseClientService, CourseClientService>();
builder.Services.AddScoped<ILessonClientService, LessonClientService>();
builder.Services.AddScoped<IAuthorClientService, AuthorClientService>();
builder.Services.AddScoped<IAuthorApplicationClientService, AuthorApplicationClientService>();
builder.Services.AddScoped<IEnrolledUserClientService, EnrolledUserClientService>();
builder.Services.AddScoped<IAiClientService, AiClientService>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());

await builder.Build().RunAsync();
