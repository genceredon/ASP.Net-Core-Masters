using DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositories
{
    public interface IItemRepository
    {
        public IQueryable<Item> All();
        public void Save(Item item);
        public void Delete(int id);
    }
}
