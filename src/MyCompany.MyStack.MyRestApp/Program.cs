using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace MyCompany.MyStack.MyRestApp
{
    public class Program
    {
        public static async Task Main()
        {
            var hostBuilder = CreateHostBuilder();

            await hostBuilder.Build()
                .RunAsync();
        }

        public static IWebHostBuilder CreateHostBuilder()
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host
            // The Generic Host is new in ASP.NET Core 2.1 and isn't suitable for web hosting scenarios. For web hosting scenarios, use the Web Host.
            // The Generic Host is under development to replace the Web Host in a future release and act as the primary host API in both HTTP and non-HTTP scenarios.
            // To run background task, see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
            return new WebHostBuilder()
                .UseKestrel(options => {
                    options.AddServerHeader = false;
                })
                .UseEnvironment(EnvironmentName.Development)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://*:5000")
                .ConfigureLogging(logging => {
                    logging.SetMinimumLevel(LogLevel.Warning);
                    logging.AddFilter(typeof(Program).Namespace, LogLevel.Trace);
                    logging.AddConsole();
                })
                .ConfigureServices(services => {
                    services.AddMvc()
                        .AddJsonOptions(json => {
                            json.SerializerSettings.Formatting = Formatting.Indented;
                            json.SerializerSettings.ContractResolver = new DefaultContractResolver {
                                NamingStrategy = new CamelCaseNamingStrategy {
                                    ProcessDictionaryKeys = false
                                }
                            };
                        });
                    services.AddScoped<IFooService, FooService>();
                })
                .Configure(app => {
                    app.UseDeveloperExceptionPage();
                    app.UseMvc();
                });
        }
    }
}
