using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Synuit.Idp.Admin.EntityFramework.Shared.DbContexts;
using Synuit.Idp.Admin.EntityFramework.Shared.Entities.Identity;
using Synuit.Idp.Admin.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Synuit.Idp.Admin
{
   public class Program
   {
      
      private const string SeedArgs = "/seed";

      public static async Task Main(string[] args)
      {
         Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(Configuration)
             .CreateLogger();

         try
         {
            var seed = args.Any(x => x == SeedArgs);
            if (seed) args = args.Except(new[] { SeedArgs }).ToArray();

            var host = CreateHostBuilder(args).Build();

            // Uncomment this to seed upon startup, alternatively pass in `dotnet run /seed` to seed using CLI

            //await DbMigrationHelpers.EnsureSeedData<IdentityServerConfigurationDbContext, AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, UserIdentity, UserIdentityRole>(host);
            if (seed)
            {
               await DbMigrationHelpers
                   .EnsureSeedData<IdentityServerConfigurationDbContext, AdminIdentityDbContext,
                       IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext,
                       UserIdentity, UserIdentityRole>(host);
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

      public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
         //.AddJsonFile($"appsettings.{GetAppSettingsNameFromEnv(_env)}.json", optional: true, reloadOnChange: true)
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
                  configApp.AddJsonFile("identitydata.json", optional: true, reloadOnChange: true);
                  configApp.AddJsonFile("identityserverdata.json", optional: true, reloadOnChange: true);

                  
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