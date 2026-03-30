using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering_API.Data;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Ordering_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                    new Uri(configuration["ElasticSearch:Uri"] ?? "http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = $"ordering-logs-{DateTime.UtcNow:yyyy.MM}",
                    NumberOfReplicas = 0,
                    NumberOfShards = 1,
                })
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    try
                    {
                        var orderContext = services.GetRequiredService<OrderContext>();
                        var logger = loggerFactory.CreateLogger<OrderContextSeed>();
                        OrderContextSeed.SeedAsync(orderContext, logger).Wait();
                    }
                    catch (Exception ex)
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
