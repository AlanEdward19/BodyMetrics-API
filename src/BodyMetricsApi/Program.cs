using System.Text.Json.Serialization;
using BodyMetricsApi.Features.Athletes;
using BodyMetricsApi.Features.Athletes.Create;
using BodyMetricsApi.Features.Athletes.Delete;
using BodyMetricsApi.Features.Athletes.GetAll;
using BodyMetricsApi.Features.Athletes.GetById;
using BodyMetricsApi.Features.Athletes.Import;
using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.Persistence;
using BodyMetricsApi.Features.Athletes.Update;
using BodyMetricsApi.Features.AthleteGroups.AddMember;
using BodyMetricsApi.Features.AthleteGroups.Compare;
using BodyMetricsApi.Features.AthleteGroups.Create;
using BodyMetricsApi.Features.AthleteGroups.Delete;
using BodyMetricsApi.Features.AthleteGroups.GetAll;
using BodyMetricsApi.Features.AthleteGroups.GetById;
using BodyMetricsApi.Features.AthleteGroups.RemoveMember;
using BodyMetricsApi.Features.AthleteGroups.Shared.Interfaces;
using BodyMetricsApi.Features.AthleteGroups.Shared.Persistence;
using BodyMetricsApi.Features.AthleteGroups.Update;
using BodyMetricsApi.Features.Sports;
using BodyMetricsApi.Features.Sports.Create;
using BodyMetricsApi.Features.Sports.Delete;
using BodyMetricsApi.Features.Sports.GetAll;
using BodyMetricsApi.Features.Sports.GetById;
using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Features.Sports.Shared.Persistence;
using BodyMetricsApi.Features.Sports.Update;
using BodyMetricsApi.Infrastructure.Configuration;
using BodyMetricsApi.Infrastructure.Persistence;
using BodyMetricsApi.Infrastructure.Serialization;
using BodyMetricsApi.Infrastructure.Storage;
using BodyMetricsApi.Shared.Authentication;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using MongoDB.EntityFrameworkCore.Extensions;

MongoSerializationBootstrapper.Configure();

#if DEBUG
const string LocalDebugCorsPolicy = "LocalDebugCors";
#endif

var builder = WebApplication.CreateBuilder(args);

var startupEnvironment = builder.Environment.EnvironmentName;
var mongoStartupOptions = builder.Configuration
    .GetSection(MongoDbOptions.SectionName)
    .Get<MongoDbOptions>() ?? new MongoDbOptions();
var photoStorageStartupOptions = builder.Configuration
    .GetSection(AthletePhotoStorageOptions.SectionName)
    .Get<AthletePhotoStorageOptions>() ?? new AthletePhotoStorageOptions();
var firebaseAuthenticationOptions = builder.Configuration
    .GetSection(FirebaseAuthenticationOptions.SectionName)
    .Get<FirebaseAuthenticationOptions>() ?? new FirebaseAuthenticationOptions();

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(MongoDbOptions.SectionName));
builder.Services.Configure<AthletePhotoStorageOptions>(builder.Configuration.GetSection(AthletePhotoStorageOptions.SectionName));
builder.Services.Configure<FirebaseAuthenticationOptions>(builder.Configuration.GetSection(FirebaseAuthenticationOptions.SectionName));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

#if DEBUG
builder.Services.AddCors(options =>
{
    options.AddPolicy(LocalDebugCorsPolicy, policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endif

builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = firebaseAuthenticationOptions.Issuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = firebaseAuthenticationOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = firebaseAuthenticationOptions.ProjectId,
            ValidateLifetime = true,
            NameClaimType = "user_id"
        };
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<BodyMetricsDbContext>((serviceProvider, optionsBuilder) =>
{
    var mongoOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbOptions>>().Value;
    optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
    optionsBuilder.UseMongoDB(mongoOptions.ConnectionString, mongoOptions.DatabaseName);
});

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddHostedService<MongoDbIndexesHostedService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();

builder.Services.AddScoped<ISportRepository, EfSportRepository>();
builder.Services.AddScoped<IAthleteRepository, EfAthleteRepository>();
builder.Services.AddScoped<IAthleteGroupRepository, EfAthleteGroupRepository>();

builder.Services.AddScoped<CreateSportCommandHandler>();
builder.Services.AddScoped<GetAllSportsQueryHandler>();
builder.Services.AddScoped<GetSportByIdQueryHandler>();
builder.Services.AddScoped<UpdateSportCommandHandler>();
builder.Services.AddScoped<DeleteSportCommandHandler>();

builder.Services.AddScoped<CreateAthleteCommandHandler>();
builder.Services.AddScoped<GetAllAthletesQueryHandler>();
builder.Services.AddScoped<GetAthleteByIdQueryHandler>();
builder.Services.AddScoped<ImportAthletesSpreadsheetCommandHandler>();
builder.Services.AddScoped<UpdateAthleteCommandHandler>();
builder.Services.AddScoped<DeleteAthleteCommandHandler>();

builder.Services.AddScoped<CreateAthleteGroupCommandHandler>();
builder.Services.AddScoped<GetAllAthleteGroupsQueryHandler>();
builder.Services.AddScoped<GetAthleteGroupByIdQueryHandler>();
builder.Services.AddScoped<UpdateAthleteGroupCommandHandler>();
builder.Services.AddScoped<DeleteAthleteGroupCommandHandler>();
builder.Services.AddScoped<AddAthleteToGroupCommandHandler>();
builder.Services.AddScoped<RemoveAthleteFromGroupCommandHandler>();
builder.Services.AddScoped<CompareAthleteGroupsQueryHandler>();

var photoStorageProvider = photoStorageStartupOptions.Provider;
var useInMemoryPhotoStorage = string.Equals(photoStorageProvider, "InMemory", StringComparison.OrdinalIgnoreCase);
if (useInMemoryPhotoStorage)
{
    builder.Services.AddSingleton<IAthletePhotoStorage, InMemoryAthletePhotoStorage>();
}
else
{
    builder.Services.AddSingleton<IAthletePhotoStorage, AzureBlobAthletePhotoStorage>();
}

var app = builder.Build();
var startupLogger = app.Logger;
var isDevelopmentEnvironment = app.Environment.IsDevelopment();
var isTestingEnvironment = app.Environment.IsEnvironment("Testing");
var mongoConnectionConfigured = !string.IsNullOrWhiteSpace(mongoStartupOptions.ConnectionString);
var mongoDatabaseConfigured = !string.IsNullOrWhiteSpace(mongoStartupOptions.DatabaseName);
var firebaseIssuerConfigured = !string.IsNullOrWhiteSpace(firebaseAuthenticationOptions.Issuer);
var firebaseProjectConfigured = !string.IsNullOrWhiteSpace(firebaseAuthenticationOptions.ProjectId);

startupLogger.LogInformation(
    "Startup config loaded. Environment={Environment}; MongoConfigured={MongoConfigured}; MongoDatabase={MongoDatabase}; FirebaseIssuerConfigured={FirebaseIssuerConfigured}; FirebaseProjectConfigured={FirebaseProjectConfigured}; PhotoStorageProvider={PhotoStorageProvider}; OpenApiEnabled={OpenApiEnabled}; HttpsRedirectionEnabled={HttpsRedirectionEnabled}",
    startupEnvironment,
    mongoConnectionConfigured && mongoDatabaseConfigured,
    mongoStartupOptions.DatabaseName,
    firebaseIssuerConfigured,
    firebaseProjectConfigured,
    useInMemoryPhotoStorage ? "InMemory" : "AzureBlob",
    isDevelopmentEnvironment,
    !isTestingEnvironment);

if (!mongoConnectionConfigured || !mongoDatabaseConfigured)
{
    startupLogger.LogWarning(
        "MongoDB startup config appears incomplete. ConnectionConfigured={ConnectionConfigured}; DatabaseConfigured={DatabaseConfigured}",
        mongoConnectionConfigured,
        mongoDatabaseConfigured);
}

if (!firebaseIssuerConfigured || !firebaseProjectConfigured)
{
    startupLogger.LogWarning(
        "Firebase auth startup config appears incomplete. IssuerConfigured={IssuerConfigured}; ProjectConfigured={ProjectConfigured}",
        firebaseIssuerConfigured,
        firebaseProjectConfigured);
}

if (isDevelopmentEnvironment)
{
    startupLogger.LogInformation("OpenAPI endpoint enabled for development environment.");
    app.MapOpenApi().AllowAnonymous();
}

if (!isTestingEnvironment)
{
    startupLogger.LogInformation("HTTPS redirection enabled.");
    app.UseHttpsRedirection();
}
else
{
    startupLogger.LogInformation("HTTPS redirection disabled for Testing environment.");
}

#if DEBUG
startupLogger.LogInformation("Local debug CORS policy enabled for http://localhost:5173.");
app.UseCors(LocalDebugCorsPolicy);
#endif

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
startupLogger.LogInformation("Startup pipeline configured. Application is ready to accept requests.");
app.Run();

public partial class Program;

