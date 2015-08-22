namespace BookShop.Services.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AddUserRoleBindingModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}