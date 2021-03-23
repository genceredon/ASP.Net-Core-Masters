using DomainModels;
using Serilog;
using Repositories;
using Repositories.Data;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepo;
        private readonly ILogger _logger;

        public ItemService(IItemRepository itemRepo, ILogger logger)
        {
            _itemRepo = itemRepo;
            _logger = logger;
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
                
                _logger.Information("ItemService.{methodNameName} -- Executed successfully.", nameof(GetTodoDetailsAsync));

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ItemService.{methodName} -- An error occurred.", nameof(GetTodoDetailsAsync));

                throw new Exception($"Error encountered - {ex}");
            }
        }

        public async Task<ICollection<ItemDTO>> GetAllTodoListAsync()
        {
            try
            {
                var listAllTodo = new List<ItemDTO>();
                var itemList = await _itemRepo.GetAllTodoListsAsync();

                if(itemList != null)
                {
                    listAllTodo = itemList.Select(x => new ItemDTO()
                    {
                        Id = x.Id,
                        Todo = x.Todo,
                        CreatedBy = x.CreatedBy
                    }).ToList();
                }

                _logger.Information("ItemService.{methodNameName} -- Executed successfully.", nameof(GetAllTodoListAsync));

                return listAllTodo;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ItemService.{methodName} -- An error occurred.", nameof(GetAllTodoListAsync));

                throw new Exception($"Error encountered - {ex}");
            }          
        }

        public async Task<ICollection<ItemDTO>> GetAllByFilterAsync(ItemByFilterDTO filters)
        {
            try
            {
                var filteredList = new List<ItemDTO>();
                var itemList = await _itemRepo.GetAllTodoListsAsync();

                if (itemList != null)
                {
                     filteredList = itemList
                     .Where(x => x.Todo.ToLower() == filters.Todo.ToLower())
                     .Select(f => new ItemDTO()
                     {
                         Id = f.Id,
                         Todo = f.Todo,
                         CreatedBy = f.CreatedBy
                     }).ToList();
                }

                _logger.Information("ItemService.{methodNameName} -- Executed successfully.", nameof(GetAllByFilterAsync));

                return filteredList;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ItemService.{methodName} -- An error occurred.", nameof(GetAllByFilterAsync));

                throw new Exception($"Error encountered - {ex}");
            }
        }

        public async Task<ItemResponse> AddTodoItemAsync(ItemDTO itemDto, ASPNetCoreMastersTodoListApiUser createdBy)
        {
            var itemObj = new ItemDTO();
            var result = new ItemResponse();

            try
            {
                if(itemDto != null)
                {
                    var mappedItem = itemObj.MappedObj(itemDto);

                    if(mappedItem != null || createdBy != null)
                    {
                        _logger.Information("ItemService.{methodNameName} -- Calling ItemRepository.AddTodoItemAsync.", nameof(AddTodoItemAsync));

                        result = await _itemRepo.AddTodoItemAsync(mappedItem, createdBy);

                        if(result.Status == "Error")
                        {
                            _logger.Error("ItemService.{methodNameName} -- Error encountered. {error}", nameof(AddTodoItemAsync), result.Message);

                            return result;
                        }
                    }
                }

                _logger.Information("ItemService.{methodNameName} -- Executed successfully.", nameof(AddTodoItemAsync));

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ItemService.{methodName} -- An error occurred.", nameof(AddTodoItemAsync));

                throw new Exception($"Error encountered - {ex}");
            }          
        }

        public async Task<ItemResponse> UpdateTodoItemAsync(ItemDTO itemDto)
        {
            var itemObj = new ItemDTO();
            var result = new ItemResponse();

            try
            {
                if (itemDto != null)
                {
                    var mappedItem = itemObj.MappedObj(itemDto);

                    if (mappedItem != null)
                    {
                        _logger.Information("ItemService.{methodNameName} -- Calling ItemRepository.UpdateTodoItemAsync.", nameof(UpdateTodoItemAsync));

                        result = await _itemRepo.UpdateTodoItemAsync(mappedItem);

                        if (result.Status == "Error")
                        {
                            _logger.Error("ItemService.{methodNameName} -- Error encountered. {error}", nameof(UpdateTodoItemAsync), result.Message);

                            return result;
                        }
                    }
                }

                _logger.Information("ItemService.UpdateTodoItemAsync exits successfully.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ItemService.{methodName} -- An error occurred.", nameof(UpdateTodoItemAsync));

                throw new Exception($"Error encountered - {ex}");
            }
        }

        public async Task<ItemResponse> DeleteTodoItemAsync(ItemDTO itemDto)
        {
            var itemObj = new ItemDTO();
            var result = new ItemResponse();

            try
            {
                if (itemDto != null)
                {
                    var mappedItem = itemObj.MappedObj(itemDto);

                    if (mappedItem != null)
                    {
                        _logger.Information("ItemService.{methodNameName} -- Calling ItemRepository.UpdateTodoItemAsync.", nameof(DeleteTodoItemAsync));

                        result = await _itemRepo.DeleteTodoItemAsync(mappedItem);

                        if (result.Status == "Error")
                        {
                            _logger.Error("ItemService.{methodNameName} -- Error encountered. {error}", nameof(DeleteTodoItemAsync), result.Message);

                            return result;
                        }
                    }
                }

                _logger.Information("ItemService.DeleteTodoItemAsync exits successfully.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ItemService.{methodName} -- An error occurred.", nameof(DeleteTodoItemAsync));

                throw new Exception($"Error encountered - {ex}");
            }
        }
    }
}
