using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace CalculateWeekdaysApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //.Net Core comes with a default logging provider. We have to override that 
            //if we have to use our own logger or use a 3rd party logger.

            //1. Configure the NLog here.
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Init-Start");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                logger.Error(ex, "Application Start Fail!");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads
                NLog.LogManager.Shutdown();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .ConfigureLogging(logging =>
                   {
                       logging.ClearProviders();
                       logging.SetMinimumLevel(LogLevel.Trace);
                   })
                   .UseNLog();  //Enforcing the framework to use nLog.

    }
}
