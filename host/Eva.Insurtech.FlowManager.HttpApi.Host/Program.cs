using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Eva.Insurtech.FlowManagers
{

    public static class Program
    {
        public async static Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File(
                            "Logs/logs.txt",
                            rollingInterval: RollingInterval.Day,
                            restrictedToMinimumLevel: LogEventLevel.Information
                            ))
                .WriteTo.Async(c => c.Console())
                .CreateLogger();

            try
            {
                Log.Information("Starting web host.");
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.AddAppSettingsSecretsJson()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        config.AddJsonFile("settings/appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"settings/appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                        //var configuration = config.Build();
                        //config.AddAzureAppConfiguration(configuration.GetSection("AppConfigurationConnectionString").Value);
                    })
                    .UseAutofac()
                    .UseSerilog();
                await builder.AddApplicationAsync<FlowManagerHttpApiHostModule>();
                var app = builder.Build();
                await app.InitializeApplicationAsync();
                await app.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
