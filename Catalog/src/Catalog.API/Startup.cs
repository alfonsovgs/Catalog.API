using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Catalog.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Catalog.Infrastructure;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Repositories;
using Catalog.Domain.Extensions;
using RiskFirst.Hateoas;
using Catalog.API.ResponseModels;
using Catalog.API.Controllers;
using Catalog.API.Infrastructure.Middleware;
using Catalog.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Polly;

namespace Catalog.API
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
                .AddCatalogContext(Configuration.GetSection("DataSource:ConnectionString").Value)
                .AddScoped<IItemRepository, ItemRepository>()
                .AddScoped<IArtistRepository, ArtistRepository>()
                .AddScoped<IGenreRepository, GenreRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .Addmappers()
                .AddServices()
                .AddControllers()
                .AddValidation();

            services
                .AddTokenAuthentication(Configuration)
                .AddEventBus(Configuration)
                .AddResponseCaching()
                .AddDistributedCache(Configuration);
                //.AddMemoryCache();

            services.AddLinks(config =>
            {
                config.AddPolicy<ItemHateoasResponse>(policy =>
                {
                    policy
                    .RequireRoutedLink(nameof(ItemsHateoasController.Get), nameof(ItemsHateoasController.Get))
                    .RequireRoutedLink(nameof(ItemsHateoasController.GetById), nameof(ItemsHateoasController.GetById), _ => new { id = _.Data.Id })
                    .RequireRoutedLink(nameof(ItemsHateoasController.Post), nameof(ItemsHateoasController.Post))
                    .RequireRoutedLink(nameof(ItemsHateoasController.Put), nameof(ItemsHateoasController.Put), x => new { id = x.Data.Id })
                    .RequireRoutedLink(nameof(ItemsHateoasController.Delete), nameof(ItemsHateoasController.Delete), x => new { id = x.Data.Id });
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ExcecuteMigrations(app, env);

            app
                .UseHttpsRedirection()
                .UseRouting()
                .UseMiddleware<ResponseTimeMiddlewareAsync>()
                .UseAuthentication()
                .UseAuthorization()
                .UseResponseCaching()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }

        private void ExcecuteMigrations(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Testing") return;

            var retry = Policy.Handle<SqlException>()
                .WaitAndRetry(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(6),
                    TimeSpan.FromSeconds(12),
                });

            retry.Execute(() =>
            {
                using var scope = app.ApplicationServices.CreateScope();
                scope.ServiceProvider.GetService<CatalogContext>().Database.Migrate();
            });
        }
    }
}