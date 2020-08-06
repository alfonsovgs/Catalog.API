using Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static void AddCatalogContext(this IServiceCollection services, string connectionString)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<CatalogContext>(opts =>
                {
                    opts.UseSqlServer(connectionString,
                        serverOptions => { serverOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName); });
                });
        }
    }
}
