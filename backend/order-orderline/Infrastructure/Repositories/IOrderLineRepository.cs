using order_orderline.Domain.Entites;

namespace order_orderline.Infrastructure.Repositories
{
    public interface IOrderLineRepository
    {
        Task<IEnumerable<OrderLine>> GetAllAsync();
        Task<OrderLine> GetByIdAsync(int id);
        Task CreateAsync(OrderLine orderLine);
        Task UpdateAsync(OrderLine orderLine);
        Task DeleteAsync(OrderLine orderLine);
    }
}
