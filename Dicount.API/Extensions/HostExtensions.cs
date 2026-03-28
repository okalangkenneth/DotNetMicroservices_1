using DbUp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase(this IHost host, string connectionString)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            for (int retry = 0; retry < 10; retry++)
            {
                try
                {
                    var upgrader =
                        DeployChanges.To
                            .PostgresqlDatabase(connectionString)
                            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                            .LogToConsole()
                            .Build();

                    if (upgrader.IsUpgradeRequired())
                    {
                        upgrader.PerformUpgrade();
                    }

                    return host;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "PostgreSQL not ready (attempt {Attempt}/10). Retrying in 5s...", retry + 1);
                    System.Threading.Thread.Sleep(5000);
                }
            }

            throw new Exception("Could not connect to PostgreSQL after 10 attempts.");
        }
    }
}
