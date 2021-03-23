using DomainModels;
using Repositories.Data;
using Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IItemService
    {
        public Task<ICollection<ItemDTO>> GetAllTodoListAsync();

        public Task<ICollection<ItemDTO>> GetAllByFilterAsync(ItemByFilterDTO filters);

        public Task<ItemDTO> GetTodoDetailsAsync(int itemId);

        public Task<ItemResponse> AddTodoItemAsync(ItemDTO itemDto, ASPNetCoreMastersTodoListApiUser createdBy);

        public Task<ItemResponse> UpdateTodoItemAsync(ItemDTO itemDto);

        public Task<ItemResponse> DeleteTodoItemAsync(ItemDTO itemDto);
    }
}
