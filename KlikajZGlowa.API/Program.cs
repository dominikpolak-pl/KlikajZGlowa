using KlikajZGlowa.Database;
using KlikajZGlowa.Model;
using KlikajZGlowa.Service.Interface;
using KlikajZGlowa.Service.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Text;


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Db>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // kofiguracja bazy danych 

//builder.Services.AddAuthentication()
//    .AddBearerToken(IdentityConstants.BearerScheme); // konfiguracja uwierzytelniania za pomocą tokenów Bearer

var signingKey = builder.Configuration["JWT:SigningKey"]
              ?? builder.Configuration["Jwt:SigningKey"]
              ?? throw new InvalidOperationException("Brak klucza JWT:SigningKey w appsettings.json.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"] ?? builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"] ?? builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "sub" // Dopasowane do new Claim(JwtRegisteredClaimNames.Sub, user.Id)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<Db>()
.AddSignInManager()
.AddDefaultTokenProviders(); // konfiguracja endpointów API

builder.Services.AddHttpContextAccessor(); // konfiguracja dostępu do kontekstu HTTP

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();

builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IEnrolledUserService, EnrolledUserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthorApplicationService, AuthorApplicationService>();
builder.Services.AddHttpClient<ILlmClient, LlmClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(90);
});
builder.Services.AddScoped<IAiCourseService, AiLessonService>();


builder.Services.AddScoped<ICourseService, CourseService>();


builder.Services.AddAutoMapper(
    _ => { },
    typeof(ApplicationUserService).Assembly,
    typeof(CourseService).Assembly,
    typeof(LessonService).Assembly,
    typeof(AuthorService).Assembly
    );



// Swagger / OpenAPI: simplified to avoid direct Microsoft.OpenApi dependency
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KlikajZGlowa API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Wpisz poniżej: Bearer {twój_token}"
    });

    // Zamiast options.AddSecurityRequirement używamy filtru operacji:
    options.OperationFilter<JwtSecurityRequirementsOperationFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7121",
                "http://localhost:5221")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KlikajZGlowa API v1"));
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("BlazorClient");

app.UseAuthentication();
app.UseAuthorization();

//app.MapGroup("/api/auth").MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();

public class JwtSecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
            .Union(context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>());

        var allowAnonymous = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>()
            .Union(context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>());

        if (authAttributes != null && authAttributes.Any() && (allowAnonymous == null || !allowAnonymous.Any()))
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer"),
                        new List<string>()
                    }
                }
            };
        }
    }
}