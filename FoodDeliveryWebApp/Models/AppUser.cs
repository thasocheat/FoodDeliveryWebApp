namespace FoodDeliveryWebApp.Models
{
    public class AppUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
