using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServerApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("Running with args: " + string.Join(", ", args));

            var hostBuilder = new HostBuilder()
                .ConfigureLogging(logging => {
                    // See https://msdn.microsoft.com/en-us/magazine/mt694089.aspx

                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                })
                .ConfigureServices(services => {
                    services.AddSingleton<IHostedService, MyHostedService>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}