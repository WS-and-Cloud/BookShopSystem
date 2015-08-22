namespace BookShop.Models
{
    using System;

    public class Purchase
    {
        public int Id { get; set; }

        public Guid ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public int BookId { get; set; }

        public virtual Book Book { get; set; }

        public DateTime DateOfPurchase { get; set; }

        public decimal Price { get; set; }

        public bool IsRecalled { get; set; }
    }
}