﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Domain.Repositories
{
    public interface IItemRepository : IRepository
    {
        Task<IEnumerable<Item>> GetAsync();
        Task<Item> GetAsync(Guid id);
        Item Add(Item item);
        Item Update(Item item);
    }
}