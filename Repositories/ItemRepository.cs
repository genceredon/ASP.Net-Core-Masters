using DomainModels;
using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System.Threading.Tasks;

namespace Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ASPNetCoreMastersTodoListApiContext _dataContext;
        private readonly ILogger _logger;

        public ItemRepository(ASPNetCoreMastersTodoListApiContext dataContext, ILogger logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<ICollection<Item>> GetAllTodoListsAsync()
        {
            return await _dataContext.TodoList
                .Select(x => new Item
                {
                    Id = x.Id,
                    Todo = x.Todo,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated
                }).ToListAsync();
        }

        public async Task<Item> GetTodoDetailsAsync(int id)
        {
            return await _dataContext.TodoList
                .Where(x => x.Id == id)
                .Select(x => new Item
                {
                    Id = x.Id,
                    Todo = x.Todo,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated
                }).FirstOrDefaultAsync();
        }

        public async Task<ItemResponse> AddTodoItemAsync(Item item, ASPNetCoreMastersTodoListApiUser createdBy) 
        {
            var response = new ItemResponse();
            try
            {
                if (item != null)
                {                    
                    if (!IsItemExisting(item))
                    {
                        _dataContext.TodoList.Add(new TodoList
                        { 
                            Todo = item.Todo,
                            CreatedBy = createdBy.UserName.ToString(),
                            DateCreated = DateTime.Now
                        });
                        await _dataContext.SaveChangesAsync();
                        
                        response.Status = "Success";
                        response.Message = "Todo item added successfully!.";
                        _logger.Information("{methodNameName} -- {message}", nameof(AddTodoItemAsync), response.Message);

                    }
                    else 
                    {
                        response.Status = "Error";
                        response.Message = "Todo item already exists.";
                        _logger.Error("{methodNameName} -- An error occurred. {message}", nameof(AddTodoItemAsync), response.Message);

                        return response;
                    }
                }
                else
                {
                    response.Status = "Error";
                    response.Message = "Unable to find the todo item";
                    _logger.Error("{methodName} -- An error occurred. {message}", nameof(AddTodoItemAsync), response.Message);


                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{methodName} -- An error occurred.", nameof(AddTodoItemAsync));
                throw new Exception($"Error encountered - {ex}");
            }

            return response;
        }

        public async Task<ItemResponse> UpdateTodoItemAsync(Item item)
        {
            var response = new ItemResponse();
            try
            {
                if (item != null)
                {
                    if (IsItemExisting(item))
                    {
                        var updateItem = _dataContext.TodoList.Find(item.Id);

                        if (updateItem != null)
                        {

                            updateItem.Todo = item.Todo;

                            _dataContext.TodoList.Update(updateItem);

                            await _dataContext.SaveChangesAsync();

                            response.Status = "Success";
                            response.Message = "Todo item updated successfully!.";
                            _logger.Information("{methodName} -- {message}", nameof(UpdateTodoItemAsync), response.Message);

                        }
                        else
                        {
                            response.Status = "Error";
                            response.Message = "Unable to find the todo item";
                            _logger.Error("{methodName} -- An error occurred. {message}", nameof(UpdateTodoItemAsync), response.Message);

                            return response;
                        }
                    }
                }
                else
                {
                    response.Status = "Error";
                    response.Message = "Unable to find the todo item";
                    _logger.Error("{methodName} -- An error occurred. {message}", nameof(UpdateTodoItemAsync), response.Message);

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{methodName} -- An error occurred. {0}", nameof(UpdateTodoItemAsync));
                throw new Exception($"Error encountered - {ex}");
            }
            
            return response;
        }

        public async Task<ItemResponse> DeleteTodoItemAsync(Item item)
        {
            var response = new ItemResponse();

            try
            {
                if(item != null)
                {                    
                    var todo = _dataContext.TodoList.SingleOrDefault(x => x.Id == item.Id);

                    if (todo != null)
                    {
                        _dataContext.TodoList.Remove(todo);
                        await _dataContext.SaveChangesAsync();

                        response.Status = "Success";
                        response.Message = "Todo item deleted successfully!.";
                        _logger.Information("{methodName} -- {message}", nameof(DeleteTodoItemAsync), response.Message);
                    }
                    else
                    {
                        response.Status = "Error";
                        response.Message = "Unable to find the todo item";
                        _logger.Error("{methodName} -- An error occurred. {message}", nameof(DeleteTodoItemAsync), response.Message);

                        return response;
                    }
                 }
                else
                {
                    response.Status = "Error";
                    response.Message = "Unable to find the todo item";
                    _logger.Error("{methodName} -- An error occurred. {message}", nameof(DeleteTodoItemAsync), response.Message);

                    return response;
                }
                             
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ItemRepository.DeleteTodoItemAsync -- An error occurred.");
                throw new Exception($"Error encountered - {ex}");
            }

            return response;
        }

        private bool IsItemExisting(Item item)
        {
            return _dataContext.TodoList
                        .Where(x => x.Id == item.Id || x.Todo.ToLower() == item.Todo.ToLower())
                        .Any();
        }
    }
}
