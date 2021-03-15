using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IItemService
    {
        public Task<IEnumerable<ItemDTO>> GetAllAsync();

        public Task<IEnumerable<ItemDTO>> GetAllByFilterAsync(ItemByFilterDTO filters);

        public Task<ItemDTO> GetAsync(int itemId);

        public void Add(ItemDTO itemDto);

        public void Update(ItemDTO itemDto);

        public void Delete(int id);


    }
}
