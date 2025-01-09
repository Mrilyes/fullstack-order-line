using Microsoft.EntityFrameworkCore;
using order_orderline.Domain.Entites;
using order_orderline.Infrastructure.Data;

namespace order_orderline.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync() =>
            await _context.Orders.Include(o => o.OrderLines).ToListAsync();

        public async Task<Order?> GetByIdAsync(int id) =>
            await _context.Orders.Include(o => o.OrderLines).FirstOrDefaultAsync(o => o.OrderId == id);

        public async Task CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Order order)
        {
           
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            
        }
    }
}
