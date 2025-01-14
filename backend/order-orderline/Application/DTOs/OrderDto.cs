namespace order_orderline.Application.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; } = null!;

        public List<OrderLineDto>? OrderLines { get; set; }
    }
}
