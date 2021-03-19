using DomainModels;
using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ASPNetCoreMastersTodoListApiContext _dataContext;

        public ItemRepository(ASPNetCoreMastersTodoListApiContext dataContext)
        {
            _dataContext = dataContext;
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
                    }
                    else 
                    {
                        response.Status = "Error";
                        response.Message = "Todo item already exists.";

                        return response;
                    }
                }
                else
                {
                    response.Status = "Error";
                    response.Message = "Unable to find the todo item";

                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error encountered - {ex}");
            }

            return response;
        }

        public async Task<ItemResponse> UpdateTodoItemAsync(Item item)
        {
            var response = new ItemResponse();

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
                    }
                    else
                    {
                        response.Status = "Error";
                        response.Message = "Unable to find the todo item";

                        return response;
                    }
                }
            }
            else
            {
                response.Status = "Error";
                response.Message = "Unable to find the todo item";

                return response;
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
                    }
                    else
                    {
                        response.Status = "Error";
                        response.Message = "Unable to find the todo item";

                        return response;
                    }
                 }
                else
                {
                    response.Status = "Error";
                    response.Message = "Unable to find the todo item";

                    return response;
                }
                             
            }
            catch (Exception ex)
            {
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
