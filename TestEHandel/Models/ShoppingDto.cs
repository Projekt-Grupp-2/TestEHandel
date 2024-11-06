namespace TestEHandel.Models
{
    public class ShoppingDto
    {

        public Guid ShoppingId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();


    }
}
