using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;


namespace ASPNetCoreMastersTodoList
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Project", "Session 10-Homework")
            .Enrich.WithProperty("Environment", "Local")
            .WriteTo.Seq("http://localhost:5341/")
            .CreateLogger();
           
            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
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

        private static void AddAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment;
            config.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true,
                    reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                    optional: true);

            if (env.IsDevelopment())
            {
                config.AddUserSecrets<Program>();
            }
        }
    }
}
