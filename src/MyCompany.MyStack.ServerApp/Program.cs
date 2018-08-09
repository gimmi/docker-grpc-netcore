using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyCompany.MyStack.ServerApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("Running with args: " + string.Join(", ", args));

            var hostBuilder = new HostBuilder()
                .ConfigureLogging(logging => {
                    // See https://msdn.microsoft.com/en-us/magazine/mt694089.aspx

                    logging.SetMinimumLevel(LogLevel.Warning);
                    logging.AddFilter(typeof(Program).Namespace, LogLevel.Trace);
                    logging.AddConsole();
                })
                .ConfigureServices(services => {
                    services.AddSingleton<MyStackServerImpl>();
                    services.AddSingleton<IHostedService, GrpcHostedService>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}