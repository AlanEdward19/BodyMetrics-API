using System.Text.Json.Serialization;
using BodyMetricsApi.Features.Athletes;
using BodyMetricsApi.Features.Athletes.Create;
using BodyMetricsApi.Features.Athletes.Delete;
using BodyMetricsApi.Features.Athletes.GetAll;
using BodyMetricsApi.Features.Athletes.GetById;
using BodyMetricsApi.Features.Athletes.Shared.Interfaces;
using BodyMetricsApi.Features.Athletes.Shared.Persistence;
using BodyMetricsApi.Features.Athletes.Update;
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
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

MongoSerializationBootstrapper.Configure();

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(MongoDbOptions.SectionName));
builder.Services.Configure<AthletePhotoStorageOptions>(builder.Configuration.GetSection(AthletePhotoStorageOptions.SectionName));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<BodyMetricsDbContext>((serviceProvider, optionsBuilder) =>
{
    var mongoOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbOptions>>().Value;
    optionsBuilder.UseMongoDB(mongoOptions.ConnectionString, mongoOptions.DatabaseName);
});

builder.Services.AddScoped<ISportRepository, EfSportRepository>();
builder.Services.AddScoped<IAthleteRepository, EfAthleteRepository>();

builder.Services.AddScoped<CreateSportCommandHandler>();
builder.Services.AddScoped<GetAllSportsQueryHandler>();
builder.Services.AddScoped<GetSportByIdQueryHandler>();
builder.Services.AddScoped<UpdateSportCommandHandler>();
builder.Services.AddScoped<DeleteSportCommandHandler>();

builder.Services.AddScoped<CreateAthleteCommandHandler>();
builder.Services.AddScoped<GetAllAthletesQueryHandler>();
builder.Services.AddScoped<GetAthleteByIdQueryHandler>();
builder.Services.AddScoped<UpdateAthleteCommandHandler>();
builder.Services.AddScoped<DeleteAthleteCommandHandler>();

var photoStorageProvider = builder.Configuration[$"{AthletePhotoStorageOptions.SectionName}:Provider"];
if (string.Equals(photoStorageProvider, "InMemory", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IAthletePhotoStorage, InMemoryAthletePhotoStorage>();
}
else
{
    builder.Services.AddSingleton<IAthletePhotoStorage, AzureBlobAthletePhotoStorage>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;

