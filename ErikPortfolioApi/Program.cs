using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;
using System;
using System.IO;

namespace ErikPortfolioApi
{
    public class Program
    {
        private static readonly string EnvironmentName;
        private static readonly IConfiguration Configuration;

        static Program()
        {
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static int Main(string[] args)
        {
            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            MigrateDatabase();

            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddConfiguration(Configuration);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                    .UseSerilog();
        }

        private static void MigrateDatabase()
        {
            try
            {
                var cnx = new NpgsqlConnection(Configuration.GetConnectionString("db"));
                var evolve = new Evolve.Evolve(cnx, msg => Log.Information(msg))
                {
                    Locations = new[] { "db/migrations" },
                    IsEraseDisabled = true
                };

                evolve.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error("Database migration failed.", ex);
                throw;
            }
        }
    }
}
