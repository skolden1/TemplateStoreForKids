namespace MallarEmelieMVC.Data.Model
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
