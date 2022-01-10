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

            ////Intégration de SWAGGER pour la documentation de l'API
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "DIONYSOS API",
            //        Version = "v1",
            //        Description = "API de l'application Dionysos",
            //        Contact = new OpenApiContact
            //        {
            //            Name = "Dionysos",
            //            Email = string.Empty
            //        },
            //    });
            //});
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

            //// Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //// specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI3(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DIONYSOS API V1");

            //    // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
            //    c.RoutePrefix = string.Empty;
            //});

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
