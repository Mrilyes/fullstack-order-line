using order_orderline.Application.DTOs;

namespace order_orderline.Application.Services
{
    public interface IOrderLineService
    {
        Task<IEnumerable<OrderLineDto>> GetAllOrderLinesAsync();
        Task<OrderLineDto> GetOrderLineByIdAsync(int id);
        Task<OrderLineDto> AddOrderLineAsync(OrderLineDto orderLineDto);
        Task UpdateOrderLineAsync(int id, OrderLineDto orderLineDto);
        Task<bool> DeleteOrderLineAsync(int id);
    }
}
