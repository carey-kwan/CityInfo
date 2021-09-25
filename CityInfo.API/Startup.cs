using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Services;
using CityInfo.API.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;

namespace CityInfo.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw ArgumentNullException(nameof(configuration));
        }

        private Exception ArgumentNullException(string v)
        {
            throw new NotImplementedException();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {   //services in .NET Core is a component intended for common consumption in an application
            //framework services examples are MVC, EF Core
            //application service examples are components that send mail
            //built-in services are ApplicationBuilder and WebHostEnvironment

            services.AddControllers().AddNewtonsoftJson();

            services.AddMvc()       //this registers MVC related services on our container. 
                .AddMvcOptions(o =>
                {
                    o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                });
            //by default asp.net core serializes and deserializes to json

#if DEBUG
            //NOTES: AddTransient means service is created each time it is requested for lightweight, stateless services
            //AddScoped means service is created once per request 
            //AddSingleton means service is created first time it is requested.  every subsequent request will use the same instance
            services.AddTransient<IMailService, LocalMailService>();
#else   
            services.AddTransient<IMailService, CloudMailService>();
#endif
            var connectionstring = _configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o =>
            {
                o.UseSqlServer(connectionstring);
            });

            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    //allows input of assemblies which will automatically get scanned for files that contain mapping configurations.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();   //this displays a simpler, status page in the browser
            app.UseRouting();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllers();
            });
        }


    }
}
