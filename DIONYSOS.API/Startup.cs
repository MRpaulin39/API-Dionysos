using DIONYSOS.API.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;

namespace DIONYSOS.API
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddMvc();

            //Configuration de la connexion à la BDD
            var connectionString = _config["ConnectionStrings:DionysosConnexionString"];
            services.AddDbContext<DionysosContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "DIONYSOS API";
                    document.Info.Description = "API de l'application Dionysos";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Dionysos",
                        Email = string.Empty,
                        Url = "http://dionysos.com"
                    };
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseOpenApi();
            app.UseSwaggerUi3(c =>
            {
                c.Path = string.Empty;
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
