using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HystrixCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Person _person;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,Person person)
        {
            _logger = logger;
            _person = person;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            //ProxyGeneratorBuilder builder=new ProxyGeneratorBuilder();
            //using (IProxyGenerator generator=builder.Build())
            //{
            //    Person p = generator.CreateClassProxy<Person>();
            //    await p.HelloAsync("rock");
            //}
            //await  _person.HelloAsync("rock");
             _person.Add(2,4);
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
