using System;
using System.ComponentModel.DataAnnotations;
using DomainModels;

namespace Services.DTO
{
    public class ItemByFilterDTO
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [StringLength(128, MinimumLength = 1)]
        public string Todo { get; set; }

        public string CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public Item MappedObj(ItemDTO item)
        {
            Todo = item.Todo;

            var dtoItemObj = new Item()
            {
                Id = Id,
                Todo = Todo,
                CreatedBy = CreatedBy,
                DateCreated = DateCreated
            };

            return dtoItemObj;
        }
    }
}
