using System.ComponentModel.DataAnnotations;
using DomainModels;

namespace Services.DTO
{
    public class ItemDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 1)]
        public string Todo { get; set; }

        public Item MappedObj(ItemDTO item)
        {
          
            var dtoItemObj = new Item()
            {
                Id = item.Id,
                Todo = item.Todo
            };

            return dtoItemObj;
        }
    }
}
