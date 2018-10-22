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
        private readonly ILogger<MyStackServerImpl> _logger;

        public MyStackServerImpl(ILogger<MyStackServerImpl> logger)
        {
            _logger = logger;
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
            var responseDelay = TimeSpan.FromMilliseconds(10);
            var payloadSizeBytes = 3 * 1024;

            _logger.LogInformation("=> ServerStream({})", request.Message);
            while (!context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var ticks = Stopwatch.GetTimestamp();

                    await responseStream.WriteAsync(new ServerStreamResponse {
                        Message = $"Message #{ticks}",
                        Payload = ByteString.CopyFrom(new byte[payloadSizeBytes]),
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

        public override async Task BidiStream(IAsyncStreamReader<BidiStreamRequest> requestStream, IServerStreamWriter<BidiStreamResponse> responseStream, ServerCallContext context)
        {
            BidiStreamRequest request = null;
            int i = 0;
            while (await requestStream.MoveNext(context.CancellationToken))
            {
                if (request == null)
                {
                    request = requestStream.Current;
                    _logger.LogInformation("Start of the subscription");
                }
                await responseStream.WriteAsync(new BidiStreamResponse {
                    Message = $"response #{++i}",
                    Payload = ByteString.CopyFrom(new byte[3 * 1024])
                });
            }
        }
    }
}