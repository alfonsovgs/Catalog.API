using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Catalog.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddCatalogContext(this IServiceCollection services, string connectionString)
        {
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<CatalogContext>(opts =>
                {
                    opts.UseSqlServer(
                        connectionString,
                        serverOptions =>
                        {
                            var assemby = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                            serverOptions.MigrationsAssembly(assemby);
                        });
                });

            return services;
        }
    }
}
