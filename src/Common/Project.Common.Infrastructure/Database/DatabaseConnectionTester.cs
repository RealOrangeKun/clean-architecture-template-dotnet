using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Project.Common.Infrastructure.Database;


public static class DatabaseConnectionTester
{
    public static async Task<WebApplication> TestDatabaseConnectionsOnStartup(
        this WebApplication app,
        params Type[] dbContextTypes)
    {
        if (dbContextTypes == null || dbContextTypes.Length == 0)
        {
            throw new ArgumentException("At least one DbContext type must be provided", nameof(dbContextTypes));
        }

        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        try
        {
            ILogger logger = services.GetRequiredService<ILogger<DbContext>>();
            logger.LogInformation("Testing database connections on startup...");

            foreach (Type contextType in dbContextTypes)
            {
                if (!typeof(DbContext).IsAssignableFrom(contextType))
                {
                    throw new ArgumentException($"Type {contextType.Name} is not a DbContext type");
                }

                logger.LogInformation("Testing connection for {DbContextName}...", contextType.Name);

                var dbContext = (DbContext)services.GetRequiredService(contextType);

                if (!await dbContext.Database.CanConnectAsync())
                {
                    throw new Exception($"Cannot connect to database using {contextType.Name}");
                }

                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1 AS TestQuery");
                logger.LogInformation("Database connection for {DbContextName} verified successfully", contextType.Name);
            }

            logger.LogInformation("All database connections tested successfully!");
        }
        catch (Exception ex)
        {
            ILogger logger = services.GetRequiredService<ILogger<DbContext>>();
            logger.LogError(ex, "An error occurred while testing database connections on startup");

            Environment.Exit(1);
        }


        return app;
    }
}
