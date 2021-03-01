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
        public string Text { get; set; }

        public Item MappedObj(ItemDTO item)
        {
            Text = item.Text;

            var dtoItemObj = new Item()
            {
                Id = Id,
                Text = Text
            };

            return dtoItemObj;
        }
    }
}
