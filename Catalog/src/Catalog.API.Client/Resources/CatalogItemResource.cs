using System;
using System.Threading;
using System.Threading.Tasks;
using Catalog.API.Client.Base;
using Catalog.API.Contract.Item;

namespace Catalog.API.Client.Resources
{
    public class CatalogItemResource : ICatalogItemResource
    {
        private readonly IBaseClient _client;

        public CatalogItemResource(IBaseClient client)
        {
            _client = client;
        }

        private Uri BuildUri(Guid id, string path = "")
        {
            return _client.BuildUri(string.Format("api/items/{0}", id, path));
        }

        public async Task<ItemResponse> Get(Guid id, CancellationToken cancellationToken = default)
        {
            var uri = BuildUri(id);
            return await _client.GetAsync<ItemResponse>(uri, cancellationToken);
        }
    }
}