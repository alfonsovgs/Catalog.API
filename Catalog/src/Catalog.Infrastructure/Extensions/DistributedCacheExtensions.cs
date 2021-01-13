using System.Threading.Tasks;
using Catalog.Domain.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Catalog.Infrastructure.Extensions
{
    public static class DistributedCacheExtensions
    {
        private static readonly JsonSerializerSettings _serializerSettings = CreateSettings();

        public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(CacheSettings));
            var settingsTyped = settings.Get<CacheSettings>();
            services.Configure<CacheSettings>(settings);
            services.AddDistributedRedisCache(opts => opts.Configuration = settingsTyped.ConnectionString);
            return services;
        }

        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);
            return json is null ? default : JsonConvert.DeserializeObject<T>(json, _serializerSettings);
        }

        public static async Task SetObjectAsync(this IDistributedCache cache, string key, object value)
        {
            var json = JsonConvert.SerializeObject(value, _serializerSettings);
            await cache.SetStringAsync(key, json);
        }

        public static async Task SetObjectAsync(this IDistributedCache cache, string key, object value,
            DistributedCacheEntryOptions options)
        {
            var json = JsonConvert.SerializeObject(value, _serializerSettings);
            await cache.SetStringAsync(key, json, options);
        }

        private static JsonSerializerSettings CreateSettings() => new JsonSerializerSettings();
    }
}