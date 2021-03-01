using DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly DataContext _dataContext;

        public ItemRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IQueryable<Item> All()
        {
            return _dataContext.itemList.AsQueryable();
        }

        public void Save(Item item) 
        {
            try
            {
                if (item != null)
                {
                    var isItemExisting = _dataContext.itemList.Any(x => x.Id == item.Id || x.Text.ToLower() == item.Text.ToLower());
                    
                    if (!isItemExisting)
                    {
                        //add new item
                        var lastId = _dataContext.itemList.Max(x => x.Id);
                        _dataContext.itemList.Add(new Item { Id = lastId + 1, Text = item.Text });
                    }
                    else 
                    {
                        //update existing item
                         _dataContext.itemList.Where(x => x.Id == item.Id).Select(i => { i.Text = item.Text; return i; }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error encountered - {ex}");
            }
        }

        public void Delete(int id) 
        {
            try
            {
                var item = _dataContext.itemList.SingleOrDefault(x => x.Id == id);
                if(item != null)
                {
                    _dataContext.itemList.Remove(item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error encountered - {ex}");
            }
        }
    }
}
