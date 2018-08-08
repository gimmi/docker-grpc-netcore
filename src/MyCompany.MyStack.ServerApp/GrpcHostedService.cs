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
            const int port = 50052;

            _logger.LogInformation("Starting Grpc server on port {}", port);
            GrpcEnvironment.SetLogger(new LogLevelFilterLogger(new ConsoleLogger(), Grpc.Core.Logging.LogLevel.Debug));
            
            _server = new Server {
                Services = { MyStackServer.BindService(_myStackServerImpl) },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            };
            _server.Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopped");
            await _server.ShutdownAsync();
        }
    }
}