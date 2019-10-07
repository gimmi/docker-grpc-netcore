using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyCompany.MyStack.ClientWorkerApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureLogging(logging => {
                    logging.SetMinimumLevel(LogLevel.Warning);
                    logging.AddFilter(typeof(Program).Namespace, LogLevel.Trace);
                    logging.AddConsole();
                })
                .UseDefaultServiceProvider(serviceProvider => {
                    serviceProvider.ValidateScopes = true;
                    serviceProvider.ValidateOnBuild = true;
                })
                .ConfigureServices(services => {
                    services.AddHostedService<Worker>();
                });

            using var host = hostBuilder.UseConsoleLifetime().Build();

            // TODO Services initialization here

            await host.StartAsync();
            await host.WaitForShutdownAsync();

            // TODO Services shutdown here
        }
    }
}
