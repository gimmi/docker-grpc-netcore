using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
                return Ok(JsonConvert.DeserializeObject(json));
            }
            else
            {
                return NotFound(new { Error = "No config found for " + moduleInstanceId });
            }
        }
    }
}
