using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ClientApp
{
    public class ClientHostedService : IHostedService
    {
        private readonly ILogger<ClientHostedService> _logger;
        private readonly MyStackServer.MyStackServerClient _myStackServerClient;
        private CancellationTokenSource _stopTokenSrc;
        private Task[] _tasks;

        public ClientHostedService(ILogger<ClientHostedService> logger, MyStackServer.MyStackServerClient myStackServerClient)
        {
            _logger = logger;
            _myStackServerClient = myStackServerClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");

            _stopTokenSrc = new CancellationTokenSource();

            _tasks = new[] {
                //Task.Run(() => WorkerAsync(_stopTokenSrc.Token), cancellationToken),
                Task.Run(() => ServerStreamWorkerAsync(_stopTokenSrc.Token), cancellationToken),
                //Task.Run(() => BidiStreamWorkerAsync(_stopTokenSrc.Token), cancellationToken),
            };

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");

            _stopTokenSrc.Cancel();

            return Task.WhenAll(_tasks);
        }

        private async Task WorkerAsync(CancellationToken cancellationToken)
        {
            var counter = 0;

            using (var response = _myStackServerClient.ServerStream(new ServerStreamRequest { Message = "Please start stream" }))
            {
                using (var responseStream = response.ResponseStream)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var request = new EchoRequest { Message = $"msg #{++counter}" };
                        var rr = await _myStackServerClient.EchoAsync(request);
                        await Console.Out.WriteLineAsync($"Echo({request.Message}) => {rr.Message}");

                        try
                        {
                            await _myStackServerClient.FailAsync(new FailRequest { Message = "AHHHH" });
                        }
                        catch (RpcException ex)
                        {
                            await Console.Out.WriteLineAsync(ex.ToString());
                        }

                        await Task.Delay(3000, cancellationToken);
                    }
                }
            }
        }

        private async Task ServerStreamWorkerAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("<= ServerStream()");
                    var request = new ServerStreamRequest { Message = "Please start stream" };
                    using (var responseStream = _myStackServerClient.ServerStream(request, cancellationToken: cancellationToken))
                    {
                        while (await responseStream.ResponseStream.MoveNext(cancellationToken))
                        {
                            var response = responseStream.ResponseStream.Current;
                            var payloadSizeKb = response.Payload.Length / 1024;
                            var elapsedTicks = Stopwatch.GetTimestamp() - response.Timestamp;
                            var elapsedMs = (double)elapsedTicks / (Stopwatch.Frequency / 1000);

                            await Console.Out.WriteLineAsync($"=> {payloadSizeKb} KB {elapsedMs} ms");
                        }
                        _logger.LogInformation("No more data from server");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error");
                }
            }
        }

        private async Task BidiStreamWorkerAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var conn = _myStackServerClient.BidiStream())
                {
                    await conn.RequestStream.WriteAsync(new BidiStreamRequest { Message = "" });
                    while (await conn.ResponseStream.MoveNext(cancellationToken))
                    {
                        var resp = conn.ResponseStream.Current;

                        await Console.Out.WriteLineAsync(resp.Message);

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await conn.RequestStream.WriteAsync(new BidiStreamRequest());
                        }
                    }

                    await conn.RequestStream.CompleteAsync();
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Client shutdown");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogWarning(ex, "Server shutdown");
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