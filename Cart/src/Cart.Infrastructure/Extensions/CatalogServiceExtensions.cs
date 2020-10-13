using Cart.Domain.Services;
using Cart.Infrastructure.Extensions.Policies;
using Cart.Infrastructure.Services;
using Catalog.API.Client;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cart.Infrastructure
{
    public static class CatalogServiceExtensions
    {
        public static IServiceCollection AddCatalogService(this IServiceCollection services, Uri uri)
        {
            services.AddScoped<ICatalogService, CatalogService>();

            services.AddHttpClient<ICatalogClient, CatalogClient>(client => client.BaseAddress = uri)
                .SetHandlerLifetime(TimeSpan.FromMinutes(2))
                .AddPolicyHandler(CatalogServicePolicies.RetryPolicy())
                .AddPolicyHandler(CatalogServicePolicies.CircuitBreakerPolicy());


            return services;
        }
    }
}