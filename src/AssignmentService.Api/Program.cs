using AssignmentService.Application.Services;
using AssignmentService.Infrastructure;
using AssignmentService.Infrastructure.Persistence;
using AssignmentService.Infrastructure.Repositories;
using AssignmentService.Application.Interfaces;
using AssignmentService.Application.Validators;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<AssignmentService.Application.Services.AssignmentService>();

builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<SubmissionService>();

builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();
builder.Services.AddScoped<EvaluationService>();

builder.Services.AddControllers();


//API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Assignment Service API", Version = "v1", Description = "Mikrostoritev za oddajo in ocenjevanje nalog" });
});
builder.Services.AddOpenApi();


// Add global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


// Add FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateAssignmentDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateAssignmentDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateSubmissionDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSubmissionDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<UpsertEvaluationDtoValidator>();

builder.Services.AddFluentValidationAutoValidation();


// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);


var app = builder.Build();

// Database migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Exception handling
app.UseExceptionHandler();


//API documentation
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assignment Service API v1");

    c.RoutePrefix = string.Empty;
});

// Health checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message
            }),
            duration = report.TotalDuration
        });
        await context.Response.WriteAsync(result);
    }
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();