using CalculateWeekdaysApi.Models.ApiResponse;
using CalculateWeekdaysApi.Models.Entities;
using CalculateWeekdaysApi.Repositories;
using CalculateWeekdaysApi.Repositories.Implementations;
using CalculateWeekdaysApi.Services;
using CalculateWeekdaysApi.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace CalculateWeekdaysApi
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
            services.AddControllers();


            services.AddScoped<IWeekdayService, WeekdayService>();

            services.AddScoped<IServiceApiRepository, ServiceApiRepository>();

            services.AddSingleton(new ApiConfiguration(Configuration["ApiConfig:ApiKey"], Configuration["ApiConfig:ApiBaseUrl"], Configuration["ApiConfig:ApiQueryParam"]));

            services.AddScoped<IServiceDbRepository, ServiceDbRepository>();

            //Set 5 min as the lifetime for the HttpMessageHandler objects in the pool used for the Catalog Typed Client
            services.AddHttpClient<IServiceApiRepository, ServiceApiRepository>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddDbContext<HolidayContext>(opts => opts.UseSqlServer(Configuration["ConnectionString:HolidaysDb"]));


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeekdaysApi", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            //Add Swagger to Middleware.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeekdaysApi");
                c.RoutePrefix = "swagger";

            });
        }
    }
}
