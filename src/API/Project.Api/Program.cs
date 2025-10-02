using Project.Common.Presentation.Endpoints;
using Project.Common.Infrastructure;
using Project.Common.Infrastructure.Configuration;
using System.Reflection;
using Project.Api.Middlewares;
using Project.Common.Application;
using Project.Api.Extensions;
using Project.Modules.Users.Infrastructure;
using Project.Modules.Users.Infrastructure.Database;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Project.Common.Infrastructure.Email;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddProblemDetailsWithExtensions();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<DatabaseExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
;

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerDocumentation();
builder.Services.AddOpenApiDocumentation();


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
string redisConnectionString = builder.Configuration.GetConnectionStringOrThrow("Redis");
string fromEmail = builder.Configuration.GetValueOrThrow<string>($"{FluentEmailOptions.SectionName}:From");

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString, name: "database", tags: ["ready"]);

Action<IRegistrationConfigurator>[] configureConsumersActions = [
    UsersModule.ConfigureConsumers];

InfrastructureOptions infrastructureOptions = builder.Configuration.BuildInfrastructureOptions(
    builder.Logging,
    configureConsumersActions);

builder.Services.AddInfrastructure(infrastructureOptions);

builder.Services.AddUsersModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwagger();
    app.UseSwaggerUIWithOpenApi();
}

Type[] dbContextTypes = [
    typeof(UsersDbContext)];

// Uncomment the following line to test database connections on startup
// await app.TestDatabaseConnectionsOnStartup(dbContextTypes);

app.UseCors("AllowAll");

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapEndpoints();


await app.RunAsync();

public partial class Program;