using Catalog.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Catalog.Domain;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Catalog.InfrastructureSp
{
    public class ItemRepository : IItemsRepository
    {
        private readonly SqlConnection _sqlConnection;

        public ItemRepository(string conenctionString)
        {
            _sqlConnection = new SqlConnection(conenctionString);
        }

        public async Task<IEnumerable<Item>> GetAsync()
        {
            var result = await _sqlConnection
                .QueryAsync<Item>("GetAllitems", commandType: CommandType.StoredProcedure);

            return result.AsList();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            return await _sqlConnection.ExecuteScalarAsync<Item>("GetAllItems", new {Id = id.ToString()},
                commandType: CommandType.StoredProcedure);
        }

        public Item Add(Item item)
        {
            var restult = _sqlConnection
                    .ExecuteScalar<Item>("InsertItem", item, commandType: CommandType.StoredProcedure);
            return restult;
        }

        public Item Update(Item item)
        {
            var result = _sqlConnection
                .ExecuteScalar<Item>("Updateitem", item, commandType: CommandType.StoredProcedure);
            return result;
        }

        public Item Delete(Item item)
        {
            throw new NotImplementedException();
        }
    }
}