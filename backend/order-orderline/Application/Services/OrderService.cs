using AutoMapper;
using Microsoft.EntityFrameworkCore;
using order_orderline.Application.DTOs;
using order_orderline.Domain.Entites;
using order_orderline.Infrastructure.Data;
using order_orderline.Infrastructure.Repositories;

// adding the improvement code in chatgpt
// frontend api integr
namespace order_orderline.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public OrderService(IOrderRepository repository, AppDbContext context, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _repository.GetAllAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task AddOrderAsync( OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            order.OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}";

            await _repository.CreateAsync(order);
        }

        public async Task UpdateOrderAsync(int id, OrderDto orderDto)
        {
            var existingOrder = await _repository.GetByIdAsync(id);

            if (existingOrder == null) throw new Exception("Order not found");

            // Update simple properties
            existingOrder.CustomerName = orderDto.CustomerName;
            existingOrder.OrderDate = orderDto.OrderDate;

            // Update OrderLines
            var updatedOrderLines = orderDto.OrderLines;

            // Remove order lines that are no longer present
            var linesToRemove = existingOrder.OrderLines
                .Where(ol => !updatedOrderLines.Any(uol => uol.OrderLineId == ol.OrderLineId))
                .ToList();

            foreach (var lineToRemove in linesToRemove)
            {
                // 1) Remove it from the parent's collection
                existingOrder.OrderLines.Remove(lineToRemove);

                // 2) Also explicitly remove it from the context
                _context.OrderLines.Remove(lineToRemove);
            }

            // Update existing order lines and add new ones
            foreach (var updatedLine in updatedOrderLines)
            {
                var existingLine = existingOrder.OrderLines
                    .FirstOrDefault(ol => ol.OrderLineId == updatedLine.OrderLineId);

                if (existingLine != null)
                {
                    // Update existing line
                    existingLine.ArticleId = updatedLine.ArticleId;
                    existingLine.ProductName = updatedLine.ProductName;
                    existingLine.Quantity = updatedLine.Quantity;
                    existingLine.Price = updatedLine.Price;
                }
                else
                {
                    // Add new line
                    existingOrder.OrderLines.Add(new OrderLine
                    {
                        ArticleId = updatedLine.ArticleId,
                        ProductName = updatedLine.ProductName,
                        Quantity = updatedLine.Quantity,
                        Price = updatedLine.Price
                    });
                }
            }

            await _repository.UpdateAsync(existingOrder);
        }



        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return false;

            await _repository.DeleteAsync(order);
            return true;
        }
    }
}
