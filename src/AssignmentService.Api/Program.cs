using AssignmentService.Application.Services;
using AssignmentService.Infrastructure;
using AssignmentService.Infrastructure.Persistence;
using AssignmentService.Infrastructure.Repositories;
using AssignmentService.Application.Interfaces;
using AssignmentService.Application.Validators;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<AssignmentService.Application.Services.AssignmentService>();

builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<SubmissionService>();

builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();
builder.Services.AddScoped<EvaluationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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


var app = builder.Build();

// Database migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();