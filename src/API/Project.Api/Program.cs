using Project.Common.Presentation.Endpoints;
using Project.Common.Infrastructure;
using Project.Common.Infrastructure.Configuration;
using Project.Common.Infrastructure.Database;
using System.Reflection;
using Project.Api.Middlewares;
using Project.Common.Application;
using Project.Api.Extensions;
using Project.Modules.Users.Infrastructure;
using Project.Modules.Users.Infrastructure.Database;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

Assembly[] moduleApplicationAssemblies = [
    Project.Modules.Users.Application.AssemblyReference.Assembly];

builder.Services.AddApplication(moduleApplicationAssemblies);

string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString, name: "database", tags: ["ready"]);

builder.Services.AddInfrastructure(databaseConnectionString);

builder.Services.AddUsersModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Type[] dbContextTypes = [
    typeof(UsersDbContext)];

await app.TestDatabaseConnectionsOnStartup(dbContextTypes);

app.UseCors("AllowAll");

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapEndpoints();


app.Run();

public partial class Program;