using Catalog.API.Contract.Item;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.API.Client.Resources
{
    public interface ICatalogItemResource
    {
        Task<ItemResponse> Get(Guid id, CancellationToken cancellationToken = default);
    }
}