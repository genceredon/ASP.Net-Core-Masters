using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreMastersTodoList.Api.ApiModels
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
