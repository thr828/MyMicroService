using System;
using System.Linq;
using System.Net.Http;
using Consul;

namespace ServiceConsumer
{
    /// <summary>
    /// 服务发现
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            using (var consulClient=new ConsulClient(configuration =>
                {
                    configuration.Address =new Uri("http://192.168.1.102:8500");
                    configuration.Datacenter = "dc1";
                }))
            {
                {
                    Console.WriteLine("所有注册的服务");
                    var services = consulClient.Agent.Services().Result.Response.Values;
                    foreach (var s in services)
                    {
                        Console.WriteLine($"{s.ID},{s.Service},{s.Address},{s.Port}");
                    }
                }
                {
                    Console.WriteLine("某个服务");
                    var services = consulClient.Agent.Services().Result.Response.Values
                        .Where(p=>p.Service.Equals("ProductService",StringComparison.OrdinalIgnoreCase));
                    Console.WriteLine($"找到{services.Count()}条");
                    //负载均衡
                    Random rad=new Random();
                    int index = rad.Next(services.Count());
                    var service = services.ElementAt(index);
                    Console.WriteLine($"http://{service.Address}:{service.Port}/WeatherForecast");

                    Console.WriteLine("调用服务");
                    using (HttpClient http=new HttpClient())
                    {
                     var response=   http.GetAsync($"http://{service.Address}:{service.Port}/WeatherForecast").Result;
                     Console.WriteLine(response.StatusCode);
                    }
                }

            }
        }
    }
}
