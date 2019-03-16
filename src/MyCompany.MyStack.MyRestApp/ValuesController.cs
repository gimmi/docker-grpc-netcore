using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyCompany.MyStack.MyRestApp
{
    public class ValuesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFooService _fooService;
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;

        public ValuesController(ILogger<ValuesController> logger, AppConfig appConfig, IHostingEnvironment hostingEnvironment, IFooService fooService)
        {
            _hostingEnvironment = hostingEnvironment;
            _fooService = fooService;
            _logger = logger;
            _appConfig = appConfig;
        }

        [HttpGet("api/exception")]
        public object GetException()
        {
            throw new ApplicationException("AHHH");
        }

        [HttpGet("api/values")]
        public object GetValues()
        {
            _fooService.LogSomething();
            return new
            {
                _hostingEnvironment.ApplicationName,
                _hostingEnvironment.EnvironmentName,
                _hostingEnvironment.ContentRootPath
            };
        }

        [HttpGet("api/v1/config")]
        public async Task<ActionResult> GetConfigAsync([FromQuery(Name = "module-instance-id")] string moduleInstanceId)
        {
            if (string.IsNullOrWhiteSpace(moduleInstanceId))
            {
                return BadRequest(new { Error = "Missing required query parameter: module-instance-id" });
            }
            var cfgPath = Path.Combine(_appConfig.DataDir, moduleInstanceId + ".json");
            if (System.IO.File.Exists(cfgPath))
            {
                var json = await System.IO.File.ReadAllTextAsync(cfgPath, Encoding.UTF8);
                return Json(JsonConvert.DeserializeObject<JObject>(json));
            }
            else
            {
                return Json(new object());
            }
        }

        [HttpGet("api/jsonserialization")]
        public object GetJsonSerialization()
        {
            return new {
                PascalCaseProperty = "value",
                camelCaseProperty = "value",
                dictionary = new Dictionary<string, object> {
                    ["PascalCaseKey"] = "value",
                    ["camelCaseKey"] = "value"
                },
                array = new[]{ "value1", "value2" },
                utcDateTime = new DateTime(2018, 5, 11, 8, 20, 31, 123, DateTimeKind.Utc),
                localDateTime = new DateTime(2018, 5, 11, 8, 20, 31, 123, DateTimeKind.Local),
            };
        }
    }
}
