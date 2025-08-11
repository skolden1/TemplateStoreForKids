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
    }
}
