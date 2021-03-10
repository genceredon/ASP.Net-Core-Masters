using DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IItemRepository
    {
        public Task<IQueryable<Item>> GetAllAsync();

        public void Save(Item item);

        public void Delete(int id);
    }
}
