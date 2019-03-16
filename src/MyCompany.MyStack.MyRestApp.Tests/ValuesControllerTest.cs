using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyCompany.MyStack.MyRestApp.Tests
{
    public class ValuesControllerTest
    {
        private TestServer _server;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            var webHostBuilder = Program.CreateHostBuilder();
            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
        }

        [Test]
        public async Task Should_get_sample_data()
        {
            var resp = await _client.GetAsync("api/jsonserialization");

            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resp.Content.Headers.ContentType.ToString(), Is.EqualTo("application/json; charset=utf-8"));

            var json = await resp.Content.ReadAsStringAsync();
            Assert.That(json, new JsonEqualConstraint(@"{
                pascalCaseProperty: 'value',
                camelCaseProperty: 'value',
                dictionary: {
                    PascalCaseKey: 'value',
                    camelCaseKey: 'value'
                },
                array: [ 'value1', 'value2' ],
                utcDateTime: '2018-05-11T08:20:31.123Z',
                localDateTime: '2018-05-11T08:20:31.123'
            }"));
        }
    }
}
