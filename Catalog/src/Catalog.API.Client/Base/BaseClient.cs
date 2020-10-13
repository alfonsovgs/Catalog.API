using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Catalog.API.Client.Base
{
    public class BaseClient : IBaseClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public BaseClient(HttpClient client, string baseUrl)
        {
            _client = client;
            _baseUrl = baseUrl;
        }

        public async Task<T> GetAsync<T>(Uri uri, CancellationToken cancellationToken)
        {
            var result = await _client.GetAsync(uri, cancellationToken);
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public Uri BuildUri(string format) =>
            new UriBuilder(_baseUrl)
            {
                Path = format
            }.Uri;
    }
}