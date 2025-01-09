using Microsoft.EntityFrameworkCore;
using order_orderline.Domain.Entites;
using order_orderline.Infrastructure.Data;

namespace order_orderline.Infrastructure.Repositories
{
    public class OrderLineRepository : IOrderLineRepository
    {
        private readonly AppDbContext _context;

        public OrderLineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderLine>> GetAllAsync()
        {
            return await _context.OrderLines.Include(ol => ol.Article).ToListAsync();
        }

        public async Task<OrderLine> GetByIdAsync(int id)
        {
            return await _context.OrderLines.Include(ol => ol.Article).FirstOrDefaultAsync(ol => ol.OrderLineId == id);
        }

        public async Task CreateAsync(OrderLine orderLine)
        {
            await _context.OrderLines.AddAsync(orderLine);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderLine orderLine)
        {
            // Fetch the existing order line from the database
            var existingOrderLine = await _context.OrderLines
                .FirstOrDefaultAsync(ol => ol.OrderLineId == orderLine.OrderLineId);

            if (existingOrderLine == null)
            {
                throw new InvalidOperationException("OrderLine does not exist.");
            }

            // Update the properties but exclude primary key changes
            _context.Entry(existingOrderLine).CurrentValues.SetValues(orderLine);
            // Ensure OrderLineId is not marked as modified
            _context.Entry(existingOrderLine).Property(ol => ol.OrderLineId).IsModified = false;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(OrderLine orderLine)
        {
            _context.OrderLines.Remove(orderLine);
            await _context.SaveChangesAsync();
        }
    }
}
