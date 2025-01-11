using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using order_orderline.Application.DTOs;
using order_orderline.Application.Services;
using order_orderline.Domain.Entites;
using order_orderline.Infrastructure.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderOrderline.Tests.Services
{
    public class OrderServiceTests
    {
        //  ---> GetAllOrdersAsync Tests <---
        //Scenarios to Test
        //Happy Path:  Returns a list of OrderDto.
        //Empty List: Returns an empty list.
        //Null Response: Handles null from the repository.
        //Exception Handling: Handles exceptions thrown by the repository.

        [Fact]
        // Scenario: the repository returns a list of Order objects. / The service maps these objects to a list of OrderDto objects and returns them.
        // Purpose:  Calls the repository’s GetAllAsync method. / Maps the returned Order objects to OrderDto objects using IMapper. / Returns the correct list of OrderDto objects. 
        public async Task GetAllOrdersAsync_ShouldReturnListOfOrderDtos()
        {
            // Arrange
            // 1. Create mock objects for dependencies
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // 2. Set up mock data
            var orders = new List<Order>
            {
                new Order { OrderId = 1, CustomerName = "Customer 1", OrderNumber = "ORD-20231010120000" },
                new Order { OrderId = 2, CustomerName = "Customer 2", OrderNumber = "ORD-20231010120001" }
            };

            var orderDtos = new List<OrderDto>
            {
                new OrderDto { OrderId = 1, CustomerName = "Customer 1", OrderNumber = "ORD-20231010120000" },
                new OrderDto { OrderId = 2, CustomerName = "Customer 2", OrderNumber = "ORD-20231010120001" }
            };

            // 3. Configure mock repository to return the test data
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);

            // 4. Configure mock mapper to return the mapped DTOs
            mockMapper.Setup(mapper => mapper.Map<List<OrderDto>>(orders)).Returns(orderDtos);

            // 5. Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act
            // 6. Call the method under test
            var result = await service.GetAllOrdersAsync();

            // Assert
            // 7. Verify the result is not null
            Assert.NotNull(result);

            // 8. Verify the result contains the expected number of items
            Assert.Equal(2, result.Count);

            // 9. Verify the mock repository was called once
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);

            // 10. Verify the mock mapper was called once
            mockMapper.Verify(mapper => mapper.Map<List<OrderDto>>(orders), Times.Once);
        }

        [Fact]
        // Scenario: The repository returns an empty list of orders.
        // Purpose: Ensure the service handles empty results correctly.
        public async Task GetAllOrdersAsync_ShouldReturnEmptyList()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Set up mock data (empty list)
            var orders = new List<Order>();
            var orderDtos = new List<OrderDto>();

            // Configure mock repository to return the empty list
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);

            // Configure mock mapper to return the empty list of DTOs
            mockMapper.Setup(mapper => mapper.Map<List<OrderDto>>(orders)).Returns(orderDtos);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act
            var result = await service.GetAllOrdersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<List<OrderDto>>(orders), Times.Once);
        }

        [Fact]
        //Scenario: The repository returns null.
        //Purpose: Ensure the service handles null results gracefully.
        public async Task GetAllOrdersAsync_ShouldHandleNullResponse()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to return null
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync((List<Order>)null);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act
            var result = await service.GetAllOrdersAsync();

            // Assert
            Assert.Null(result);
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            // Verify the mapper was never called
            mockMapper.Verify(mapper => mapper.Map<List<OrderDto>>(It.IsAny<List<Order>>()), Times.Never);
        }

        [Fact]
        //Scenario: The repository throws an exception.
        //Purpose: Ensure the service handles exceptions gracefully.
        public async Task GetAllOrdersAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to throw an exception
            mockRepo.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetAllOrdersAsync());
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<List<OrderDto>>(It.IsAny<List<Order>>()), Times.Never);
        }


        // ---> GetOrderByIdAsync Tests <---
        //Scenarios to Test
            //Happy Path: Returns an OrderDto for a valid order ID.   
            //Null Response: Handles null when the order is not found.
            //Exception Handling: Handles exceptions thrown by the repository.

        // Scenario: The repository returns a valid Order.
        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrderDto()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Set up mock data
            var order = new Order { OrderId = 1, CustomerName = "Customer 1", OrderNumber = "ORD-20231010120000" };
            var orderDto = new OrderDto { OrderId = 1, CustomerName = "Customer 1", OrderNumber = "ORD-20231010120000" };

            // Configure mock repository to return the test data
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);

            // Configure mock mapper to return the mapped DTO
            mockMapper.Setup(mapper => mapper.Map<OrderDto>(order)).Returns(orderDto);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act
            var result = await service.GetOrderByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Customer 1", result.CustomerName);
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<OrderDto>(order), Times.Once);
        }

        // Scenario: The repository returns null (order not found).
        [Fact]
        public async Task GetOrderByIdAsync_ShouldHandleNullResponse()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to return null
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Order)null);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act
            var result = await service.GetOrderByIdAsync(1);

            // Assert
            Assert.Null(result);
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }

        // Scenario: The repository throws an exception.
        [Fact]
        public async Task GetOrderByIdAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to throw an exception
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetOrderByIdAsync(1));
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }


        // ---> AddOrderAsync Tests <---
            //Scenarios to Test
                //Happy Path: The order is successfully created.
                //Null Input: The input OrderDto is null.
                //Exception Handling: The repository throws an exception.
    
        // Scenario: The order is successfully created.
        [Fact]
        public async Task AddOrderAsync_ShouldCreateOrder()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Set up mock data
            var orderDto = new OrderDto { CustomerName = "Customer 1", OrderDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var order = new Order { CustomerName = "Customer 1", OrderDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            // Configure mock mapper to return the mapped entity
            mockMapper.Setup(mapper => mapper.Map<Order>(orderDto)).Returns(order);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act
            await service.AddOrderAsync(orderDto);

            // Assert
            mockRepo.Verify(repo => repo.CreateAsync(order), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<Order>(orderDto), Times.Once);
        }

        // Scenario: The input OrderDto is null.
        [Fact]
        public async Task AddOrderAsync_ShouldThrowExceptionWhenOrderDtoIsNull()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddOrderAsync(null));
            mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Order>()), Times.Never);
            mockMapper.Verify(mapper => mapper.Map<Order>(It.IsAny<OrderDto>()), Times.Never);
        }

        // Scenario: The repository throws an exception.
        [Fact]
        public async Task AddOrderAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Set up mock data
            var orderDto = new OrderDto { CustomerName = "Customer 1", OrderDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            var order = new Order { CustomerName = "Customer 1", OrderDate = DateOnly.FromDateTime(DateTime.UtcNow) };

            // Configure mock mapper to return the mapped entity
            mockMapper.Setup(mapper => mapper.Map<Order>(orderDto)).Returns(order);

            // Configure mock repository to throw an exception
            mockRepo.Setup(repo => repo.CreateAsync(order)).ThrowsAsync(new Exception("Database error"));

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.AddOrderAsync(orderDto));
            mockRepo.Verify(repo => repo.CreateAsync(order), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<Order>(orderDto), Times.Once);
        }


        // ---> UpdateOrderAsync Tests <---

        //Happy Path: The order is successfully updated.
        //Order Not Found: The repository returns null (order not found).
        //Null Input: The input OrderDto is null.
        //Exception Handling: The repository throws an exception.


        //  Scenario: The order is successfully updated.
        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrder()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Set up mock data
            var existingOrder = new Order
            {
                OrderId = 1,
                CustomerName = "Customer 1",
                OrderNumber = "ORD-20231010120000",
                OrderLines = new List<OrderLine>()
            };

            var orderDto = new OrderDto
            {
                OrderId = 1,
                CustomerName = "Updated Customer",
                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                OrderLines = new List<OrderLineDto>()
            };

            // Configure mock repository to return the existing order
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingOrder);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act
            await service.UpdateOrderAsync(1, orderDto);

            // Assert
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockRepo.Verify(repo => repo.UpdateAsync(existingOrder), Times.Once);
        }

        // Scenario: The repository returns null (order not found).
        [Fact]
        public async Task UpdateOrderAsync_ShouldThrowExceptionWhenOrderNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to return null
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Order)null);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => service.UpdateOrderAsync(1, new OrderDto()));

            // Verify the exception message
            Assert.Equal("Order not found", exception.Message);

            // Verify the repository and mapper were never called
            mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Never);
            mockMapper.Verify(mapper => mapper.Map(It.IsAny<OrderDto>(), It.IsAny<Order>()), Times.Never);
        }

        // Scenario: The input OrderDto is null.
        [Fact]
        public async Task UpdateOrderAsync_ShouldThrowExceptionWhenOrderDtoIsNull()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockMapper = new Mock<IMapper>();

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, mockMapper.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateOrderAsync(1, null));

            // Verify the exception message
            Assert.Contains("OrderDto cannot be null.", exception.Message);

            // Verify the repository and mapper were never called
            mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Never);
            mockMapper.Verify(mapper => mapper.Map(It.IsAny<OrderDto>(), It.IsAny<Order>()), Times.Never);



        }

        // ---> DeleteOrderAsync Tests <---
        //Happy Path: The order is successfully deleted.
        //Order Not Found: The repository returns null (order not found).
        //Exception Handling: The repository throws an exception.

        // Scenario: The order is successfully deleted.
        [Fact]
        public async Task DeleteOrderAsync_ShouldDeleteOrder()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();

            // Set up mock data
            var order = new Order { OrderId = 1, CustomerName = "Customer 1", OrderNumber = "ORD-20231010120000" };

            // Configure mock repository to return the existing order
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, null);

            // Act
            var result = await service.DeleteOrderAsync(1);

            // Assert
            Assert.True(result);
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockRepo.Verify(repo => repo.DeleteAsync(order), Times.Once);
        }

        // Scenario: The repository returns null (order not found).
        [Fact]
        public async Task DeleteOrderAsync_ShouldReturnFalseWhenOrderNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();

            // Configure mock repository to return null
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Order)null);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, null);

            // Act
            var result = await service.DeleteOrderAsync(1);

            // Assert
            Assert.False(result);
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Order>()), Times.Never);
        }

        // Scenario: The repository throws an exception.
        [Fact]
        public async Task DeleteOrderAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();

            // Set up mock data
            var order = new Order { OrderId = 1, CustomerName = "Customer 1", OrderNumber = "ORD-20231010120000" };

            // Configure mock repository to return the existing order
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);

            // Configure mock repository to throw an exception
            mockRepo.Setup(repo => repo.DeleteAsync(order)).ThrowsAsync(new Exception("Database error"));

            // Create an instance of the service with the mocked dependencies
            var service = new OrderService(mockRepo.Object, null, null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeleteOrderAsync(1));

            // Verify the repository was called
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockRepo.Verify(repo => repo.DeleteAsync(order), Times.Once);
        }
    }
}
