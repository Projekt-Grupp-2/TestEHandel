namespace TestEHandel.Models
{
    public class ShoppingUpdateDto
    {


        public Guid UserId { get; set; }
        public List<CartItemUpdateDto> UpdatedItems { get; set; } = new List<CartItemUpdateDto>();



    }
}
