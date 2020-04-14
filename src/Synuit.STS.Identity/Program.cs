using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace Synuit.Idp
{
   public class Program
   {
      public static void Main(string[] args)
      {
         Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(Configuration)
             .CreateLogger();
         try
         {
            CreateHostBuilder(args).Build().Run();
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

      public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .Build();

      public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostContext, configApp) =>
               {
                  //configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                  //configApp.AddJsonFile($"appsettings.{GetAppSettingsNameFromEnv(hostContext.HostingEnvironment.EnvironmentName)}.json", optional: true, reloadOnChange: true);
                  configApp.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);

                  if (hostContext.HostingEnvironment.IsDevelopment())
                  {
                     configApp.AddUserSecrets<Startup>();
                  }

                  configApp.AddEnvironmentVariables();
                  configApp.AddCommandLine(args);
               })
              .ConfigureWebHostDefaults(webBuilder =>
              {
                 webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                 webBuilder.UseStartup<Startup>();
              })
              .UseSerilog((hostContext, loggerConfig) =>
              {
                 loggerConfig
                       .ReadFrom.Configuration(hostContext.Configuration)
                       .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
              });

      public static string GetAppSettingsNameFromEnv(string env)
      {
         env = env.ToLower();
         if (env == "development" || env == "dev")
         {
            return "appsettings.dev.json";
         }
         else if (env == "production" || env == "prod")
         {
            return "appsettings.prod.json";
         }
         else
         { //stage or test
            return "appsettings." + env.ToLower() + ".json";
         }
      }
   }
}