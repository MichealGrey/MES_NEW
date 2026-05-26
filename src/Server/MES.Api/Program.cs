using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MES.Services.Production;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Register Production Service (singleton for in-memory V1)
builder.Services.AddSingleton<IProductionService, ProductionService>();

var app = builder.Build();

// Seed production data
using (var scope = app.Services.CreateScope())
{
    var productionService = scope.ServiceProvider.GetRequiredService<IProductionService>();
    await productionService.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
