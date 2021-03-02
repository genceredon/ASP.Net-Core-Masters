using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepo;

        public ItemService(IItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public ItemDTO Get(int itemId)
        {
            try
            {
                var result = new ItemDTO();
                var item = _itemRepo.All().FirstOrDefault(x => x.Id == itemId);

                if (item != null)
                {
                    result.Id = item.Id;
                    result.Text = item.Text;
                }

                return result;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error encountered - {ex}");
            }       
        }

        public IEnumerable<ItemDTO> GetAll()
        {
            var itemList = _itemRepo.All();
            var listAll = itemList.Select(x => new ItemDTO()
            {
                Id = x.Id,
                Text = x.Text
            }).ToList();

            return listAll;
        }

        public IEnumerable<ItemDTO> GetAllByFilter(ItemByFilterDTO filters)
        {
            var itemList = _itemRepo.All();

            var filteredList = itemList.Where(x => x.Text.ToLower() == filters.Text.ToLower()).Select(f => new ItemDTO()
            {
                Id = f.Id,
                Text = f.Text
            }).ToList();

            return filteredList;
        }

        public void Add(ItemDTO itemDto)
        {
            var itemObj = new ItemDTO();
            var result = itemObj.MappedObj(itemDto);

            _itemRepo.Save(result);
        }

        public void Update(ItemDTO itemDto)
        {
            var itemObj = new ItemDTO();
            var result = itemObj.MappedObj(itemDto);
            var validId = IsIdExisting(itemDto.Id);

            if (validId)
            {
                _itemRepo.Save(result);
            }
            else
            {
                throw new ArgumentException($"{itemDto.Id} Id not found.");
            }
        }

        public void Delete(int id)
        {
            var validId = IsIdExisting(id);

            if (validId)
            {
                _itemRepo.Delete(id);
            }
            else
            {
                throw new ArgumentException($"{id} Id not found.");
            }
        }

        private bool IsIdExisting(int id)
        {
            bool isExisting;
            var itemExisting = Get(id);

            if (itemExisting.Id != 0)
            {
                isExisting = true;
            }
            else
            {
                isExisting = false;
                return isExisting;
            }

            return isExisting;
        }
    }
}
