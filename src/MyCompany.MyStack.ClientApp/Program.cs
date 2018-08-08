using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ClientApp
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
                    
                    services.AddSingleton(sp => {
                        var channel = new Channel("localhost", 50052, ChannelCredentials.Insecure);
                        return new MyStackServer.MyStackServerClient(channel);
                    });
                    services.AddSingleton<IHostedService, ClientHostedService>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}