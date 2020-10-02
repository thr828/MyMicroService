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
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MessageService
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
            services.AddSwaggerGen(m => m.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "MessageService",
                Version = "v1"
            }));
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

            string ip = Configuration["ip"] ?? "127.0.0.1";
            int port = int.Parse(Configuration["port"] ?? "5001");
            string serviceName = "MessageService";
            string serviceId = serviceName + Guid.NewGuid();
            Console.WriteLine($"MessageService:{ip}:{port}");

            var client = new ConsulClient(option =>
            {
                option.Address = new Uri("http://192.168.1.102:8500");
                option.Datacenter = "dc1";
            });
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
                    Interval = TimeSpan.FromSeconds(15),
                    Timeout = TimeSpan.FromSeconds(5)
                }
            }).Wait();

            hostApplicationLifetime.ApplicationStopping.Register(() =>
                {
                    client.Agent.ServiceDeregister(serviceId).Wait();
                });
            app.UseSwagger();
            app.UseSwaggerUI(
                m => m.SwaggerEndpoint("/swagger/v1/swagger.json", "MessageService")
                );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
