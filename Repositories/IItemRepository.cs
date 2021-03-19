using DomainModels;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IItemRepository
    {
        public Task<Item> GetTodoDetailsAsync(int id);

        public Task<ICollection<Item>> GetAllTodoListsAsync();

        public Task<ItemResponse> AddTodoItemAsync(Item item, ASPNetCoreMastersTodoListApiUser createdBy);

        public Task<ItemResponse> UpdateTodoItemAsync(Item item);

        public Task<ItemResponse> DeleteTodoItemAsync(Item item);
    }
}
