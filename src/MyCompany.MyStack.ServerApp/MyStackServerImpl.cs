using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ServerApp
{
    public class MyStackServerImpl : MyStackServer.MyStackServerBase
    {
        private readonly ByteString Payload = ByteString.CopyFrom(new byte[3 * 1024]);

        private readonly ILogger<MyStackServerImpl> _logger;
        private readonly EventSource _eventSource;

        public MyStackServerImpl(ILogger<MyStackServerImpl> logger, EventSource eventSource)
        {
            _logger = logger;
            _eventSource = eventSource;
        }

        public override Task<EchoResponse> Echo(EchoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("=> Echo({})", request.Message);
            return Task.FromResult(new EchoResponse { Message = request.Message });
        }

        public override Task<FailResponse> Fail(FailRequest request, ServerCallContext context)
        {
            _logger.LogInformation("=> Fail({})", request.Message);
            throw new ApplicationException(request.Message);
        }

        public override async Task ServerStream(ServerStreamRequest request, IServerStreamWriter<ServerStreamResponse> responseStream, ServerCallContext context)
        {
            var responseDelay = TimeSpan.FromMilliseconds(100);

            _logger.LogInformation("=> ServerStream({})", request.Message);
            while (!context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var ticks = Stopwatch.GetTimestamp();

                    await responseStream.WriteAsync(new ServerStreamResponse {
                        Message = $"Message #{ticks}",
                        Payload = Payload,
                        Timestamp = ticks
                    });

                    var elapsedTicks = Stopwatch.GetTimestamp() - ticks;
                    var elapsed = elapsedTicks > 0 ? TimeSpan.FromTicks(elapsedTicks) : TimeSpan.Zero;
                    if (responseDelay > elapsed)
                    {
                        await Task.Delay(responseDelay.Subtract(elapsed), context.CancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error");
                }
            }
        }

        public override async Task NotificationChannel(IAsyncStreamReader<SubscriptionRequest> requestStream, IServerStreamWriter<SubscriptionResponse> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Client connected");

            while (await requestStream.MoveNext(CancellationToken.None))
            {
                var request = requestStream.Current;
                _logger.LogInformation("Subscribing to {}", request.Topic);

                _eventSource.EventPublished += OnEventPublished;
            }

            _logger.LogInformation("Client disconnected");
            _eventSource.EventPublished -= OnEventPublished;

            async void OnEventPublished(object s, EventArgs e)
            {
                _logger.LogInformation("Publishing");
                await responseStream.WriteAsync(new SubscriptionResponse {
                    Topic = "my-topic",
                    Payload = Payload,
                    Timestamp = Stopwatch.GetTimestamp()
                });
            }
        }
    }
}