using CityInfo.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;


namespace CityInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {   //this looks like a setup for a console app. because
            //a web API essentially functions as a console app

            var logger = NLogBuilder                    //cannot inject logger here, because appliacation container has not been setup yet
                .ConfigureNLog("nlog.config")           //manual declaration provides program.cs with nlog 
                .GetCurrentClassLogger();

            try
            {
                logger.Info("Initializing application ...");
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetService<CityInfoContext>();

                        //for demo purposes, delete the database and migrate up on startup so
                        //we can start with a clean slate
                        context.Database.EnsureDeleted();
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "An error occurred while migrating the database.");
                    }
                
                }

                //run the web app
                host.Run();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Application stopped because of exception.");
                throw;
            }
            finally 
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseNLog();
                });
    }
}
