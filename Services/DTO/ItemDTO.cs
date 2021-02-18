using System.ComponentModel.DataAnnotations;
using DomainModels;

namespace Services.DTO
{
    public class ItemDTO
    {
        // It is not clear on the instruction what should the class ItemDTO has 
        // I just assumed that class ItemDTO has this property
        [StringLength(128, MinimumLength = 1)]
        public string Text { get; set; }

        //7. Add DomainModels reference to Services project, then map the DTO property to DomainModel Item property

        public void MappedObj()
        {
            var dtoProp = new Item()
            {
                Items = Text
            };
        }
    }
}
