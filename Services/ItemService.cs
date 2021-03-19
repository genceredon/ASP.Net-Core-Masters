using DomainModels;
using Repositories;
using Repositories.Data;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepo;

        public ItemService(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public async Task<ItemDTO> GetTodoDetailsAsync(int itemId)
        {
            try
            {
                var result = new ItemDTO();
                var item = await _itemRepo.GetTodoDetailsAsync(itemId);

                if (item != null)
                {
                    result.Id = item.Id;
                    result.Todo = item.Todo;
                    result.CreatedBy = item.CreatedBy;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error encountered - {ex}");
            }
        }

        public async Task<IEnumerable<ItemDTO>> GetAllTodoListAsync()
        {
            var itemList = await _itemRepo.GetAllTodoListsAsync();
            
            var listAllTodo = itemList.Select(x => new ItemDTO()
            {
                Id = x.Id,
                Todo = x.Todo,
                CreatedBy = x.CreatedBy
            }).ToList();

            return listAllTodo;
        }

        public async Task<IEnumerable<ItemDTO>> GetAllByFilterAsync(ItemByFilterDTO filters)
        {
            var itemList = await _itemRepo.GetAllTodoListsAsync();

            var filteredList = itemList
                .Where(x => x.Todo.ToLower() == filters.Todo.ToLower())
                .Select(f => new ItemDTO()
                {
                    Id = f.Id,
                    Todo = f.Todo,
                    CreatedBy = f.CreatedBy
                }).ToList();

            return filteredList;
        }

        public async Task<ItemResponse> AddTodoItemAsync(ItemDTO itemDto, ASPNetCoreMastersTodoListApiUser createdBy)
        {
            var itemObj = new ItemDTO();
            var result = itemObj.MappedObj(itemDto);

            return await _itemRepo.AddTodoItemAsync(result, createdBy);
        }

        public async Task<ItemResponse> UpdateTodoItemAsync(ItemDTO itemDto)
        {
            var itemObj = new ItemDTO();
            var result = itemObj.MappedObj(itemDto);

            return await _itemRepo.UpdateTodoItemAsync(result);
        }

        public async Task<ItemResponse> DeleteTodoItemAsync(ItemDTO itemDto)
        {
            var itemObj = new ItemDTO();
            var result = itemObj.MappedObj(itemDto);

            return await _itemRepo.DeleteTodoItemAsync(result);

        }
    }
}
