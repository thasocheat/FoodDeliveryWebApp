using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebApp.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("AppUser")]
        public string? UserId { get; set; }

        public string? Status { get; set; }

        [ForeignKey("Payment")]
        public int? PaymentId { get; set; }

        public DateTime OrderAt { get; set; }

        public Product Product { get; set; }
        public AppUser AppUser { get; set; }
        public Payment Payment { get; set; }

    }
}
