using System;
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
            _logger.LogInformation("=> ServerStream({})", request.Message);

            for (int i = 0; i < 1000; i++)
            {
                var message = new ServerStreamResponse { Message = "response #" + i, Payload = ByteString.CopyFrom(new byte[3 * 1024 * 1024]) };

                _logger.LogInformation("Sending {}", message.Message);

                await responseStream.WriteAsync(message);

                await Task.Delay(10);
            }
        }
    }
}