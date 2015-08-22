namespace BookShop.Services.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}