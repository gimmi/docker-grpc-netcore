using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.MyStack.MyRestApp.Tests
{
    public class ConfigControllerTest
    {
        private TestUtils _tu;
        private TestServer _server;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _tu = new TestUtils();
            var webHostBuilder = Program.CreateHostBuilder(new AppConfig {
                DataDir = _tu.TestDir
            });
            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _tu.Dispose();
        }

        [Test]
        public async Task Should_get_config_for_module()
        {
            await File.WriteAllTextAsync(Path.Combine(_tu.TestDir, "MyModule.json"), "{ value: 123 }");
            var resp = await _client.GetAsync("api/v1/config?module-instance-id=MyModule");

            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resp.Content.Headers.ContentType.ToString(), Is.EqualTo("application/json; charset=utf-8"));

            var json = await resp.Content.ReadAsStringAsync();
            Assert.That(json, new JsonEqualConstraint("{ value: 123 }"));
        }

        [Test]
        public async Task Should_return_bad_request()
        {
            var resp = await _client.GetAsync("api/v1/config");

            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(resp.Content.Headers.ContentType.ToString(), Is.EqualTo("application/json; charset=utf-8"));

            var json = await resp.Content.ReadAsStringAsync();
            Assert.That(json, new JsonEqualConstraint("{ error: 'Missing required query parameter: module-instance-id' }"));
        }

        [Test]
        public async Task Should_return_not_found()
        {
            var resp = await _client.GetAsync("api/v1/config?module-instance-id=unknown");

            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(resp.Content.Headers.ContentType.ToString(), Is.EqualTo("application/json; charset=utf-8"));

            var json = await resp.Content.ReadAsStringAsync();
            Assert.That(json, new JsonEqualConstraint("{ error: 'No config found for unknown' }"));
        }

        [Test]
        public async Task Should_query_with_standard_client()
        {
            await File.WriteAllTextAsync(Path.Combine(_tu.TestDir, "MyModule.json"), "{ value: 123 }");
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost/");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
                
                var resp = await httpClient.GetAsync($"api/v1/config?module-instance-id={WebUtility.UrlEncode("MyModule")}");
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                dynamic config = JsonConvert.DeserializeObject(json);

                Assert.That(config.value, Is.EqualTo(123));
            }
        }
    }
}
