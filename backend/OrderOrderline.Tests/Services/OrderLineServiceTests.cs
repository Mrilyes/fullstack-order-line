using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using order_orderline.Application.DTOs;
using order_orderline.Application.Services;
using order_orderline.Domain.Entites;
using order_orderline.Infrastructure.Repositories;

namespace OrderOrderline.Tests.Services
{
    public class OrderLineServiceTests
    {
        //  ---> GetAllOrderLinesAsync Tests <---
        //Scenarios to Test
        //Happy Path:  Returns a list of OrderLineDto.
        //Empty List: Returns an empty list.
        //Null Response: Handles null from the repository.
        //Exception Handling: Handles exceptions thrown by the repository.

        [Fact]
        // Scenario: the repository returns a list of Order objects. / The service maps these objects to a list of OrderDto objects and returns them.
        // Purpose:  Calls the repository’s GetAllAsync method. / Maps the returned Order objects to OrderDto objects using IMapper. / Returns the correct list of OrderDto objects. 
        public async Task GetAllOrderLinesAsync_ShouldReturnListOfOrderLineDtos()
        {
            // Arrange
            // 1. Create mock objects for dependencies
            var mockRepo = new Mock<IOrderLineRepository>();
            var mockMapper = new Mock<IMapper>();

            // 2. Set up mock data
            var orderLines = new List<OrderLine>
            { new OrderLine
                {
                OrderLineId = 1,
                OrderId = 101,
                ArticleId = 201,
                ProductName = "Product A",
                Quantity = 2,
                Price = 10.0m
            },
                new OrderLine
                {
                OrderLineId = 2,
                OrderId = 102,
                ArticleId = 202,
                ProductName = "Product B",
                Quantity = 3,
                Price = 15.0m
                }
            };

            var orderLineDtos = new List<OrderLineDto>
            {
                new OrderLineDto
                { 
                OrderLineId = 1,
                OrderId = 101,
                ArticleId = 201,
                ProductName = "Product A",
                Quantity = 2,
                Price = 10.0m
            },
                 new OrderLineDto
                {
                OrderLineId = 2,
                OrderId = 102,
                ArticleId = 202,
                ProductName = "Product B",
                Quantity = 3,
                Price = 15.0m
                }
            };

            // 3. Configure mock repository to return the test data
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orderLines);

            // 4. Configure mock mapper to return the mapped DTOs
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<OrderLineDto>>(orderLines)).Returns(orderLineDtos);

            // 5. Create an instance of the service with the mocked dependencies
            var service = new OrderLineService(mockRepo.Object, mockMapper.Object);

            // Act
            // 6. Call the method under test
            var result = await service.GetAllOrderLinesAsync();

            // Assert
            // 7. Verify the result is not null
            Assert.NotNull(result);

            // 8. Verify the result contains the expected number of items
            Assert.Equal(2, result.Count());

            // 9. Verify the mock repository was called once
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);

            // 10. Verify the mock mapper was called once
            mockMapper.Verify(mapper => mapper.Map<IEnumerable<OrderLineDto>>(orderLines), Times.Once);
        }

        [Fact]
        // Scenario: The repository returns an empty list of orders.
        // Purpose: Ensure the service handles empty results correctly.
        public async Task GetAllOrderLinesAsync_ShouldReturnEmptyList()
        {
            // Arrange
            var mockRepo = new Mock<IOrderLineRepository>();
            var mockMapper = new Mock<IMapper>();

            // Set up mock data (empty list)
            var orderLines = new List<OrderLine>();
            var orderLineDtos = new List<OrderLineDto>();

            // Configure mock repository to return the empty list
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orderLines);

            // Configure mock mapper to return the empty list of DTOs
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<OrderLineDto>>(orderLines)).Returns(orderLineDtos);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderLineService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await service.GetAllOrderLinesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<IEnumerable<OrderLineDto>>(orderLines), Times.Once);
        }

        [Fact]
        //Scenario: The repository returns null.
        //Purpose: Ensure the service handles null results gracefully.
        public async Task GetAllOrderLinesAsync_ShouldHandleNullResponse()
        {
            // Arrange
            var mockRepo = new Mock<IOrderLineRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to return null
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync((IEnumerable<OrderLine>)null);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderLineService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await service.GetAllOrderLinesAsync();

            // Assert
            Assert.Null(result);
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            // Verify the mapper was never called
            mockMapper.Verify(mapper => mapper.Map<IEnumerable<OrderLineDto>>(It.IsAny<List<OrderLine>>()), Times.Never);
        }

        [Fact]
        //Scenario: The repository throws an exception.
        //Purpose: Ensure the service handles exceptions gracefully.
        public async Task GetAllOrderLinesAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderLineRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to throw an exception
            mockRepo.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // Create an instance of the service with the mocked dependencies
            var service = new OrderLineService(mockRepo.Object, mockMapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetAllOrderLinesAsync());
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()), Times.Never);
        }


        // ---> GetOrdeLinerByIdAsync Tests <---
        //Scenarios to Test
        //Happy Path: Returns an OrderLineDto for a valid order line ID.   
        //Null Response: Handles null when the order is not found.
        //Exception Handling: Handles exceptions thrown by the repository.

        // Scenario: The repository returns a valid Order.
        [Fact]
        public async Task GetOrderLineByIdAsync_ShouldReturnOrderLineDto()
        {
            // Arrange
            var mockRepo = new Mock<IOrderLineRepository>();
            var mockMapper = new Mock<IMapper>();

            // Set up mock data
            var orderLine = new OrderLine
            {
                OrderLineId = 1,
                OrderId = 101,
                ArticleId = 201,
                ProductName = "Product A",
                Quantity = 2,
                Price = 10.0m
            };
            var orderLineDto = new OrderLineDto
            {
                OrderLineId = 1,
                OrderId = 101,
                ArticleId = 201,
                ProductName = "Product A",
                Quantity = 2,
                Price = 10.0m
            };

            // Configure mock repository to return the test data
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(orderLine);

            // Configure mock mapper to return the mapped DTO
            mockMapper.Setup(mapper => mapper.Map<OrderLineDto>(orderLine)).Returns(orderLineDto);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderLineService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await service.GetOrderLineByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Product A", result.ProductName);
            Assert.Equal(2, result.Quantity);
            Assert.Equal(10.0m, result.Price);
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<OrderLineDto>(orderLine), Times.Once);
        }

        // Scenario: The repository returns null (order not found).
        [Fact]
        public async Task GetOrderLineByIdAsync_ShouldHandleNullResponse()
        {
            // Arrange
            var mockRepo = new Mock<IOrderLineRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to return null
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((OrderLine)null);

            // Create an instance of the service with the mocked dependencies
            var service = new OrderLineService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await service.GetOrderLineByIdAsync(1);

            // Assert
            Assert.Null(result);
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<OrderLineDto>(It.IsAny<OrderLine>()), Times.Never);
        }

        // Scenario: The repository throws an exception.
        [Fact]
        public async Task GetOrderLineByIdAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var mockRepo = new Mock<IOrderLineRepository>();
            var mockMapper = new Mock<IMapper>();

            // Configure mock repository to throw an exception
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

            // Create an instance of the service with the mocked dependencies
            var service = new OrderLineService(mockRepo.Object , mockMapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetOrderLineByIdAsync(1));
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            mockMapper.Verify(mapper => mapper.Map<OrderLineDto>(It.IsAny<OrderLine>()), Times.Never);
        }

    }
}
