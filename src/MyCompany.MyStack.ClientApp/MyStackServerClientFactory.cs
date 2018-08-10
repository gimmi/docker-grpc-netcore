using Grpc.Core;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ClientApp
{
    public class MyStackServerClientFactory
    {
        private const string Target = "serverapp.mystack.mycompany:50052";

        private readonly ILogger<MyStackServerClientFactory> _logger;

        public MyStackServerClientFactory(ILogger<MyStackServerClientFactory> logger)
        {
            _logger = logger;
        }

        public MyStackServer.MyStackServerClient Create()
        {
            _logger.LogInformation("Creating client for {}", Target);
            var channel = new Channel(Target, ChannelCredentials.Insecure);
            return new MyStackServer.MyStackServerClient(channel);
        }
    }
}