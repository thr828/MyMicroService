using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace PorductService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(m =>
            {
                m.SwaggerDoc("v1",new OpenApiInfo()
                {
                    Title = "ProductService",
                    Version = "v1"
                });
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IHostApplicationLifetime hostApplicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            string ip=Configuration["ip"]??"127.0.0.1";
            int port = int.Parse(Configuration["port"] ?? "5002");
            string serviceName = "ProductService";
            string serviceId = serviceName + Guid.NewGuid();
            Console.WriteLine($"ProductService:{ip}:{port}");

            var client = new ConsulClient(option =>
            {
                option.Address = new Uri("http://192.168.1.102:8500");
                option.Datacenter = "dc1";

            });
            //服务注册到consul
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = serviceName,
                Address = ip,
                Port = port,
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                    HTTP = $"http://{ip}:{port}/api/Health",
                    Interval = TimeSpan.FromSeconds(5),
                    Timeout = TimeSpan.FromSeconds(10),
                },
            }).Wait();

            //应用程序结束
            hostApplicationLifetime.ApplicationStopping.Register(() =>
                {
                    client.Agent.ServiceDeregister(serviceId).Wait();
                });
            app.UseSwagger();
            app.UseSwaggerUI(
                m => m.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductService"));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
