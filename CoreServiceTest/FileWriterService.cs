using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CoreServiceTest
{
    public class FileWriterService : IHostedService, IDisposable
    {
        private const string Path = @"C:\Users\mberka\Desktop\TestApplication.txt";

        private readonly ILogger logger = null;
        private Timer timer;
        private readonly IConfiguration configuration = null;
    
        public FileWriterService(ILogger<FileWriterService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            WriteTimeToFile("Started!");
            this.timer = new Timer(
                DoWork,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            logger.LogInformation("Info log");
            logger.LogInformation(configuration["prefix"]);
            logger.LogDebug("Debug log");
            logger.LogCritical("Critical log");
            logger.LogError("Error log");
            logger.LogWarning("Warning log");
            using (logger.BeginScope("Starting scope", 25))
            {
                logger.LogInformation("Checking object state");

                logger.LogInformation("Updating object reference");

                logger.LogError("Something went wrong");
            }
            WriteTimeToFile("tick");
        }

        private void WriteTimeToFile(string message)
        {
            if (!File.Exists(Path))
            {
                using (var sw = File.CreateText(Path))
                {
                    sw.WriteLine($"{DateTime.UtcNow.ToString("O")} > {message}");
                }
            }
            else
            {
                using (var sw = File.AppendText(Path))
                {
                    sw.WriteLine($"{DateTime.UtcNow.ToString("O")} > {message}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteTimeToFile("Stopped!");
            timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
