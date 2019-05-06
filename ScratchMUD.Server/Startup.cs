using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScratchMUD.Server.HostedServices;
using ScratchMUD.Server.Hubs;
using ScratchMUD.Server.Models;

namespace ScratchMUD.Server
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddHostedService<ServerTimeHostedService>();
            services.AddSingleton<EventHub>();
            services.AddTransient<PlayerContext>();
            services.AddSingleton<EditingState>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins(
                        "http://creatorsarelegion.azurewebsites.net",
                        "https://creatorsarelegion.azurewebsites.net",
                        "https://localhost:5003")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials();
            });

            app.UseHttpsRedirection();

            app.UseSignalR(routes =>
            {
                routes.MapHub<EventHub>("/EventHub");
            });

            app.UseMvc();
        }
    }
}