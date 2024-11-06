namespace TestEHandel.Models
{
    public class CartItemDto
    {


        public Guid CartItemId { get; set; } = Guid.NewGuid();

        public Guid ShoppingId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }



    }
}
