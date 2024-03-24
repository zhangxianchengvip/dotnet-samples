using Microsoft.AspNetCore.Mvc;
using Yarp.ReverseProxy.Configuration;

namespace YaprService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IProxyConfigProvider _proxyConfigProvider;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IProxyConfigProvider proxyConfigProvider)
        {
            _logger = logger;
            _proxyConfigProvider = proxyConfigProvider;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = nameof(GetYarpProxy))]
        public ActionResult GetYarpProxy()
        {
            var cfig = _proxyConfigProvider.GetConfig();
            return Ok(cfig);
        }
    }
}
