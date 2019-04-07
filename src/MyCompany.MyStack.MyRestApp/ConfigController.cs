using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MyCompany.MyStack.MyRestApp
{
    public class ConfigController : Controller
    {
        private readonly IFooService _fooService;
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;

        public ConfigController(ILogger<ValuesController> logger, AppConfig appConfig)
        {
            _logger = logger;
            _appConfig = appConfig;
        }

        [HttpGet("api/v1/config/{moduleInstanceId}")]
        public async Task<ActionResult> GetConfigAsync(string moduleInstanceId)
        {
            var cfgPath = Path.Combine(_appConfig.DataDir, moduleInstanceId + ".json");
            if (System.IO.File.Exists(cfgPath))
            {
                var json = await System.IO.File.ReadAllTextAsync(cfgPath, Encoding.UTF8);
                return Ok(JsonConvert.DeserializeObject(json));
            }

            return Ok(new JObject());
        }

        [HttpPut("api/v1/config/{moduleInstanceId}")]
        public async Task<ActionResult> PutConfigAsync(string moduleInstanceId, [FromBody] object body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cfgPath = Path.Combine(_appConfig.DataDir, moduleInstanceId + ".json");
            var json = JsonConvert.SerializeObject(body, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(cfgPath, json, Encoding.UTF8);
            return Ok();
        }

        [HttpGet("config/{moduleInstanceId}")]
        public ActionResult GetConfigUiAsync()
        {
            // ~/ match {Content Root}/wwwroot
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/index?view=aspnetcore-2.2&tabs=windows#web-root
            return File("~/config.html", "text/html");
        }
    }
}
