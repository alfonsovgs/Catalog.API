﻿using Catalog.API.Filters;
using Catalog.API.ResponseModels;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Responses;
using Catalog.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Catalog.Infrastructure.Extensions;

namespace Catalog.API.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IDistributedCache _distributedCache;

        public ItemController(IItemService itemService, IDistributedCache distributedCache)
        {
            _itemService = itemService;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int pageSize = 10, int pageIndex = 0)
        {
            var result = await _itemService.GetItemsAsync();
            
            var totalItem = result.Count();

            var itemOnpage = result
                .OrderBy(c => c.Name )
                .Skip(pageSize * pageIndex)
                .Take(pageSize);

            var model = new PaginatedItemsResponseModel<ItemResponse>(
                pageIndex, pageSize, totalItem, itemOnpage);

            return Ok(model);
        }

        [HttpGet("{id:guid}")]
        [ItemExists]
        [TypeFilter(typeof(RedisCacheFilter), Arguments = new object[] { 20 })]
        public async Task<IActionResult> GetById(Guid id)
        {
            var key = $"{typeof(ItemController).FullName}.{nameof(GetById)}.{id}";
            var cachedResult = await _distributedCache.GetObjectAsync<ItemResponse>(key);

            if(cachedResult != null)
            {
                return Ok(cachedResult);
            }

            var result = await _itemService.GetItemAsync(new GetItemRequest {Id = id});
            await _distributedCache.SetObjectAsync(key, result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddItemRequest request)
        {
            var result = await _itemService.AddItemAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [ItemExists]
        public async Task<IActionResult> Put(Guid id, EditItemRequest request)
        {
            request.Id = id;
            var result = await _itemService.EditItemAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ItemExists]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteItemRequest {Id = id};
            await _itemService.DeleteItemAsync(request);
            return NoContent();
        }
    }
}
