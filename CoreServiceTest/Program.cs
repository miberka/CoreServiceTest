using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.CommandLine;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace CoreServiceTest
{
    class Program
    {
        private static async Task Main(string[] args)
        {

            // https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddEnvironmentVariables(prefix: "TEST_");
                    if (args != null)
                    {
                        configApp.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<FileWriterService>();
                    services.AddLogging();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddEventLog();
                    logging.AddDebug();
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddEventSourceLogger();
                });
            if (isService)
            {
                await builder.RunAsServiceAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }
        }
    }
}
