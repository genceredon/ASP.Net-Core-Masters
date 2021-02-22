using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreMastersTodoList.Api.ApiModels
{
    public class ItemUpdateBindingModel

    {
        [Required]
        [StringLength(128, MinimumLength = 1)]
        public string Text { get; set; }
    }
}
