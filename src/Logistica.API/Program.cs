using Logistica.Domain.Interfaces;
using Logistica.Application.Services;
using Logistica.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure();

builder.Services.AddScoped<INormalizationOrchestrator, NormalizationOrchestratorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<Logistica.API.Middleware.GlobalExceptionMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
