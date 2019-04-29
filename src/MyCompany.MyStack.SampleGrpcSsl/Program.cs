using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Logging;
using Mycompany.Mystack;

namespace MyCompany.MyStack.SampleGrpcSssl
{
    public static class Program
    {
        public static async Task Main()
        {
            GrpcEnvironment.SetLogger(new LogLevelFilterLogger(new ConsoleLogger(), LogLevel.Debug));

            var server = await CreateServerAsync();
            server.Start();

            var client = await CreateClientAsync();
            
            var response = await client.EchoAsync(new EchoRequest {Message = "Hello World!"});
            await Console.Out.WriteLineAsync($"Client received Echo({response.Message})");

            await server.ShutdownAsync();
        }

        private static async Task<Server> CreateServerAsync()
        {
            var host = "127.0.0.1";
            var port = 40052;
            await Console.Out.WriteLineAsync($"Starting Grpc server on {host}:{port}");

            ServerCredentials credentials;

            // HTTP connection, no auth token
            // credentials = ServerCredentials.Insecure;

            // HTTPS connection, no auth token
            var certificateChain = await File.ReadAllTextAsync(@"certstrap-out\MyService.crt");
            var privateKey = await File.ReadAllTextAsync(@"certstrap-out\MyService.key");
            credentials = new SslServerCredentials(new[] {new KeyCertificatePair(certificateChain, privateKey)});

            var server = new Server {
                Services = {MyStackServer.BindService(new MyStackServerImpl())},
                Ports = {new ServerPort(host, port, credentials)}
            };

            return server;
        }

        public static async Task<MyStackServer.MyStackServerClient> CreateClientAsync()
        {
            var target = "localhost:40052";

            ChannelCredentials credentials;

            // HTTP connection, no auth token
            // credentials = ChannelCredentials.Insecure;

            // HTTPS connection, no auth token
            var rootCertificates = await File.ReadAllTextAsync(@"certstrap-out\MyCertAuth.crt");
            credentials = new SslCredentials(rootCertificates);

            // Mutual TLS
            // credentials = new SslCredentials("", new KeyCertificatePair("", ""));

            // HTTPS connection, with auth token
            // credentials = ChannelCredentials.Create(new SslCredentials(""), CallCredentials.FromInterceptor((_, metadata) => {
            //     metadata.Add("authorization", "SECRET_TOKEN");
            //     return Task.CompletedTask;
            // }));

            var channel = new Channel(target, credentials, new[] {
                new ChannelOption(ChannelOptions.SslTargetNameOverride, "MyService")
            });
            return new MyStackServer.MyStackServerClient(channel);
        }

        public class MyStackServerImpl : Mycompany.Mystack.MyStackServer.MyStackServerBase 
        {
            public override async Task<EchoResponse> Echo(EchoRequest request, ServerCallContext context)
            {
                await Console.Out.WriteLineAsync($"Server received Echo({request.Message})");
                return new EchoResponse { Message = request.Message };
            }
        }
    }
}