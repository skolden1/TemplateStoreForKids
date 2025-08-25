using MallarEmelieMVC.Areas.Identity.Models;

namespace MallarEmelieMVC.Data.Model
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentComment { get; set; } 
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public string Status { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNr { get; set; }
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
    }
}
