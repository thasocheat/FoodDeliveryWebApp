using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebApp.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public string Name { get; set; }
        public string CardNo { get; set; }
        public string ExpiryDate { get; set; }
        public int CvvNo { get; set; }
        public string Address { get; set;}
        public string PaymentMode { get; set; }
        public DateTime PayAt { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
