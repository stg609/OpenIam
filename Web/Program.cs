using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Charlie.OpenIam.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string serviceName = typeof(Program).Assembly.GetName().Name;
            BuildSerilogConfiguration(args, serviceName);

            try
            {
                Log.Logger.Information($"----- Bootstrapping {serviceName}...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "----- Host is terminated unexpectedly");
            }
            finally
            {
                Log.Logger.Warning($"----- {serviceName} is shutdown");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                     .UseSerilog();
                });

        public static IConfigurationRoot BuildSerilogConfiguration(string[] args, string serviceName)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"serilog.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .CreateLogger();

            return configuration;
        }
    }
}
