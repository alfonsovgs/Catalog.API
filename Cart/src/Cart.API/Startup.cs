using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cart.Domain.Repositories;
using Cart.Domain.Services;
using Cart.Infrastructure;
using Cart.Infrastructure.Repositories;
using Cart.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Cart.Infrastructure.BackgroundServices;
using Cart.Infrastructure.Configurations;
using Cart.Infrastructure.Extensions;
using MediatR;

namespace Cart.API
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
            services
                .AddControllers()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.IgnoreNullValues = true);

            services
                .AddScoped<ICartRepository, CartRepository>()
                .AddScoped<ICatalogService, CatalogService>()
                .AddCatalogService(new Uri(Configuration["CatalogApiUrl"]))
                .AddMediatR(AppDomain.CurrentDomain.GetAssemblies())
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddEventBus(Configuration)
                .AddHostedService<ItemSoldOutBackgroundService>()
                .Configure<CartDataSourceSettings>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseAuthorization();

            app
                .UseRouting()
                .UseHttpsRedirection()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}