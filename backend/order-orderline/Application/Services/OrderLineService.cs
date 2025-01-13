using AutoMapper;
using order_orderline.Application.DTOs;
using order_orderline.Domain.Entites;
using order_orderline.Infrastructure.Repositories;

namespace order_orderline.Application.Services
{
    public class OrderLineService : IOrderLineService
    {
        private readonly IOrderLineRepository _repository;
        private readonly IMapper _mapper;

        public OrderLineService(IOrderLineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderLineDto>> GetAllOrderLinesAsync()
        {
            var orderLines = await _repository.GetAllAsync();
            if(orderLines == null) {
                return null;
            }
            return _mapper.Map<IEnumerable<OrderLineDto>>(orderLines);
        }

        public async Task<OrderLineDto> GetOrderLineByIdAsync(int id)
        {
            var orderLine = await _repository.GetByIdAsync(id);

            if (orderLine == null)
            {
                return null;
            }

            return _mapper.Map<OrderLineDto>(orderLine);
        }

        public async Task<OrderLineDto> AddOrderLineAsync(OrderLineDto orderLineDto)
        {
            var orderLine = _mapper.Map<OrderLine>(orderLineDto);

            // Add the entity to the repository
            await _repository.CreateAsync(orderLine);

            // Map the saved entity back to a DTO with its generated ID
            return _mapper.Map<OrderLineDto>(orderLine);
        }

        public async Task UpdateOrderLineAsync(int id, OrderLineDto orderLineDto)
        {
            if (id != orderLineDto.OrderLineId)
            {
                throw new InvalidOperationException("OrderLineId mismatch between path and data.");
            }

            // Fetch the existing order line from the repository
            var existingOrderLine = await _repository.GetByIdAsync(id);
            if (existingOrderLine == null)
            {
                throw new InvalidOperationException("OrderLine not found.");
            }

            // Ensure no changes are attempted on immutable properties
            if (existingOrderLine.OrderLineId != orderLineDto.OrderLineId)
            {
                throw new InvalidOperationException("OrderLineId cannot be modified.");
            }

            // Map the updated DTO to the existing order line entity
            _mapper.Map(orderLineDto, existingOrderLine);

            // Perform the update in the repository
            await _repository.UpdateAsync(existingOrderLine);
        }


        public async Task<bool> DeleteOrderLineAsync(int id)
        {
            var existingOrderLine = await _repository.GetByIdAsync(id);
            if (existingOrderLine == null) return false;

            await _repository.DeleteAsync(existingOrderLine);
            return true;
        }
    }
}
