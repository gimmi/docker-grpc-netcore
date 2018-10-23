using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCompany.MyStack.ServerApp
{
    public class EventSourceHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly EventSource _eventSource;
        private CancellationTokenSource _stopTokenSrc;
        private Task _task;

        public EventSourceHostedService(ILogger<EventSourceHostedService> logger, EventSource eventSource)
        {
            _eventSource = eventSource;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");

            _stopTokenSrc = new CancellationTokenSource();

            _task = Task.Run(() => RandomEventGeneratorWorker(_stopTokenSrc.Token), cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");

            _stopTokenSrc.Cancel();

            await _task;
        }

        private async Task RandomEventGeneratorWorker(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _eventSource.Publish();
                    await Task.Delay(3000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
