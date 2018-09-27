using Grpc.Core;
using Microsoft.Extensions.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.ClientApp
{
    public class MyStackServerClientFactory
    {
        private readonly ILogger<MyStackServerClientFactory> _logger;
        private readonly AppSettings _appSettings;

        public MyStackServerClientFactory(ILogger<MyStackServerClientFactory> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public MyStackServer.MyStackServerClient Create()
        {
            var target = _appSettings.ServerAppTarget;

            //ChannelCredentials channelCredentials;
            //channelCredentials = ChannelCredentials.Insecure;
            //AsyncAuthInterceptor asyncAuthInterceptor = (context, metadata) => Task.CompletedTask;
            //ChannelCredentials.Create(new SslCredentials("", new KeyCertificatePair("", "")), CallCredentials.FromInterceptor(asyncAuthInterceptor));

            var channel = new Channel(target, ChannelCredentials.Insecure);
            return new MyStackServer.MyStackServerClient(channel);
        }
    }
}