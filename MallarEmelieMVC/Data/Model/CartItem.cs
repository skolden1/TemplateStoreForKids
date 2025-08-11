namespace MallarEmelieMVC.Data.Model
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public string UserId { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public int Quantity { get; set; }
    }
}
