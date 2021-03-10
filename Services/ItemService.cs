using Repositories;
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

        public async Task<ItemDTO> GetAsync(int itemId)
        {
            try
            {
                var result = new ItemDTO();
                var repoObj = await _itemRepo.GetAllAsync();

                var item = repoObj.FirstOrDefault(x => x.Id == itemId);
                
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

        public async Task<IEnumerable<ItemDTO>> GetAllAsync()
        {
            var itemList = await _itemRepo.GetAllAsync();
            var listAll = itemList.Select(x => new ItemDTO()
            {
                Id = x.Id,
                Text = x.Text
            }).ToList();

            return listAll;
        }

        public async Task<IEnumerable<ItemDTO>> GetAllByFilterAsync(ItemByFilterDTO filters)
        {
            var itemList = await _itemRepo.GetAllAsync();

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
            var itemExisting = GetAsync(id);

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
