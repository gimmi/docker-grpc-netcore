using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ServerApp
{
    public class GrpcHostedService : IHostedService
    {
        private const string Host = "0.0.0.0";
        private const int Port = 50052;

        private readonly MyStackServerImpl _myStackServerImpl;
        private readonly ILogger<GrpcHostedService> _logger;

        private Server _server;

        public GrpcHostedService(MyStackServerImpl myStackServerImpl, ILogger<GrpcHostedService> logger)
        {
            _myStackServerImpl = myStackServerImpl;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Grpc server on {}:{}", Host, Port);
            GrpcEnvironment.SetLogger(new LogLevelFilterLogger(new ConsoleLogger(), Grpc.Core.Logging.LogLevel.Debug));
            
            _server = new Server {
                Services = { MyStackServer.BindService(_myStackServerImpl) },
                Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
            };
            _server.Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopped");
            //await _server.ShutdownAsync();
            await _server.KillAsync();
        }
    }
}