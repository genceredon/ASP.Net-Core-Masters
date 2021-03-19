using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreMastersTodoList.Api.ApiModels
{
    public class ItemCreateApiModel
    {
        [Required]
        [StringLength(128, MinimumLength = 1)]
        public string Todo { get; set; }
    }
}
