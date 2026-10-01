using GestaoPedidos.Api;
using GestaoPedidos.Application;
using GestaoPedidos.Infrastructure;
using GestaoPedidos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddDbContextCheck<GestaoPedidosDbContext>();

var origensPermitidas = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? ["http://localhost:4200"];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(origensPermitidas).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<GestaoPedidosDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.PopularAsync(context);
}

app.UseCors();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
