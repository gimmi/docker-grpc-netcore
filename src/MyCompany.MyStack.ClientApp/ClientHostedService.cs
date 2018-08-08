using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ClientApp
{
    public class ClientHostedService : IHostedService
    {
        private readonly ILogger<ClientHostedService> _logger;
        private readonly MyStackServer.MyStackServerClient _myStackServerClient;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public ClientHostedService(ILogger<ClientHostedService> logger, MyStackServer.MyStackServerClient myStackServerClient)
        {
            _logger = logger;
            _myStackServerClient = myStackServerClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");
            
            _cancellationTokenSource = new CancellationTokenSource();
            
            _task = Task.Run(() => WorkerAsync(_cancellationTokenSource.Token), cancellationToken);
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            
            _cancellationTokenSource.Cancel();
            
            return _task;
        }

        private async Task WorkerAsync(CancellationToken cancellationToken)
        {
            var counter = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try 
                {
                    var response = await _myStackServerClient.EchoAsync(new EchoRequest {Message = $"MSG #{++counter}"});

                    await Console.Out.WriteLineAsync(response.Message);

                    await WaitAsync(TimeSpan.FromSeconds(3), cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during server invocation");
                    await WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
        }

        private static async Task WaitAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            try 
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException) 
            {
                // Task.Delay throw exception on cancel
            }
        }
    }
}