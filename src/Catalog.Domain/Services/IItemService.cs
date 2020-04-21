﻿using System;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Domain.Mappers;
using Catalog.Domain.Repositories;

namespace Catalog.Domain.Services
{
    public interface IItemService
    {
        Task<IEnumerable<ItemResponse>> GetItemsAsync();
        Task<ItemResponse> GetItemAsync(GetItemRequest request);
        Task<ItemResponse> AddItemAsync(AddItemRequest request);
        Task<ItemResponse> EditItemAsync(EditItemRequest request);
    }

    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemMapper _itemMapper;

        public ItemService(IItemRepository itemRepository, IItemMapper itemMapper)
        {
            _itemRepository = itemRepository;
            _itemMapper = itemMapper;
        }

        public async Task<IEnumerable<ItemResponse>> GetItemsAsync()
        {
            var result = await _itemRepository.GetAsync();
            return result.Select(i => _itemMapper.Map(i));
        }

        public async Task<ItemResponse> GetItemAsync(GetItemRequest request)
        {
            if(request?.Id == null) throw new ArgumentNullException();
            var entity = await _itemRepository.GetAsync(request.Id);

            return _itemMapper.Map(entity);
        }

        public async Task<ItemResponse> AddItemAsync(AddItemRequest request)
        {
            var item = _itemMapper.Map(request);
            var result = _itemRepository.Add(item);

            await _itemRepository.UnitOfWork.SaveEntitiesAsync();
            return _itemMapper.Map(result);
        }

        public async Task<ItemResponse> EditItemAsync(EditItemRequest request)
        {
            var existingRecord = await _itemRepository.GetAsync(request.Id);

            if (existingRecord == null)
            {
                throw new ArgumentException($"Entity with {request.Id} is not present");
            }

            var entity = _itemMapper.Map(request);
            var result = _itemRepository.Update(entity);
            return _itemMapper.Map(result);
        }
    }
}