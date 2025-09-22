using FlightBookingSystem.Data;
using FlightBookingSystem.IService;
using FlightBookingSystem.Service;
using FlightBookingSystem.Services;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.Managers;
using Betalgo.Ranul.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// OpenAI service
builder.Services.AddSingleton<IOpenAIService>(sp =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    return new OpenAIService(new OpenAIOptions { ApiKey = apiKey });
});

// Add services
builder.Services.AddScoped<IAiQueryParser, AiQueryParser>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ICsvExportService, CsvExportService>();
builder.Services.AddScoped<ICsvQueryService, CsvQueryService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))
    )
);

// FluentMigrator
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddMySql5()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(AppDbContext).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

// **Add controllers**
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

// Swagger middleware
if (builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Booking API V1");
    });
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Minimal API example
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var rng = new Random();
    var forecast = Enumerable.Range(1, 5)
        .Select(index => new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            rng.Next(-20, 56),
            summaries[rng.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
