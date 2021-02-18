using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreMastersTodoList.Api.ApiModels
{
    public class ItemCreateApiModel
    {
        //4. Add a property Text string to ItemCreateApiModel then add DataAnnotation Required and StringLength 128 with minimum 1.

        [StringLength(128, MinimumLength = 1)]
        public string Text { get; set; }
    }
}
