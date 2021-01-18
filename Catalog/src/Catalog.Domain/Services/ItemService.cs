﻿using Catalog.Domain.Configurations;
using Catalog.Domain.Mappers;
using Catalog.Domain.Repositories;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Responses;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Domain.Events;
using System.Text.Json;
using Catalog.Domain.Logging;

namespace Catalog.Domain.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemMapper _itemMapper;
        private readonly ConnectionFactory _eventBusConnectionFactory;
        private readonly ILogger<ItemService> _logger;
        private readonly EventBusSettings _settings;

        public ItemService(IItemRepository itemRepository, IItemMapper itemMapper,
            ConnectionFactory eventBusConnectionFactory,
            ILogger<ItemService> logger,
            EventBusSettings settings)
        {
            _itemRepository = itemRepository;
            _itemMapper = itemMapper;
            _eventBusConnectionFactory = eventBusConnectionFactory;
            _logger = logger;
            _settings = settings;
        }

        public async Task<IEnumerable<ItemResponse>> GetItemsAsync()
        {
            var result = await _itemRepository.GetAsync();
            return result.Select(i => _itemMapper.Map((Item) i));
        }

        public async Task<ItemResponse> GetItemAsync(GetItemRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();
            var entity = await _itemRepository.GetAsync(request.Id);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _itemMapper.Map(entity);
        }

        public async Task<ItemResponse> AddItemAsync(AddItemRequest request)
        {
            var item = _itemMapper.Map(request);
            var result = _itemRepository.Add(item);

            var modifiedRecords = await _itemRepository
                .UnitOfWork.SaveEntitiesAsync();

            _logger.LogInformation(Logging.Events.Add, Messages.NumberOfRecordAffected_modifiedRecords, modifiedRecords);
            _logger.LogInformation(Logging.Events.Add, Messages.ChangesApplied_id, result?.Id);

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

        public async Task<ItemResponse> DeleteItemAsync(DeleteItemRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _itemRepository.GetAsync(request.Id);
            result.IsInactive = true;

            _itemRepository.Update(result);
            await _itemRepository.UnitOfWork.SaveChangesAsync();

            SendDeleteMessage(new ItemSoldOutEvent { Id = request.Id.ToString() });
            return _itemMapper.Map(result);
        }

        private void SendDeleteMessage(ItemSoldOutEvent message)
        {
            try
            {
                var connection = _eventBusConnectionFactory.CreateConnection();

                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: _settings.EventQueue, true, false);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                channel.ConfirmSelect();
                channel.BasicPublish(exchange: string.Empty, routingKey: _settings.EventQueue, body: body);
                channel.WaitForConfirmsOrDie();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Unable to initialize the event bus: {message}", e.Message);
            }
        }
    }
}