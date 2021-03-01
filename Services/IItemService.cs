using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IItemService
    {
        public IEnumerable<ItemDTO> GetAll();

        public IEnumerable<ItemDTO> GetAllByFilter(ItemByFilterDTO filters);

        public ItemDTO Get(int itemId);

        public void Add(ItemDTO itemDto);

        public void Update(ItemDTO itemDto);

        public void Delete(int id);


    }
}
