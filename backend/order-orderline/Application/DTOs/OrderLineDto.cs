namespace order_orderline.Application.DTOs
{
    public class OrderLineDto
    {
        public int OrderLineId { get; set; } // Immutable
        public int OrderId { get; set; }
        public int ArticleId { get; set; }
        public string? ProductName { get; set; } // Include this in the response

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
