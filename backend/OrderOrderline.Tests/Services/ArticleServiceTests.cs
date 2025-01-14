﻿using System;
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
    public class ArticleServiceTests
    {
        public class OrderLineServiceTests
        {
            //  ---> GetAllArticleAsync Tests <---
            //Scenarios to Test
            //Happy Path:  Returns a list of ArticleDto.
            //Empty List: Returns an empty list.
            //Null Response: Handles null from the repository.
            //Exception Handling: Handles exceptions thrown by the repository.

            [Fact]
            // Scenario: the repository returns a list of Article objects. / The service maps these objects to a list of OrderDto objects and returns them.
            // Purpose:  Calls the repository’s GetAllAsync method. / Maps the returned Order objects to OrderDto objects using IMapper. / Returns the correct list of OrderDto objects. 
            public async Task GetAllArticleAsync_ShouldReturnListOfOrderLineDtos()
            {
                // Arrange
                // 1. Create mock objects for dependencies
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // 2. Set up mock data
                var article = new List<Article>
                {
                    new ()  { ArticleId = 1, Name = "Article A", Price = 10.0m },
                    new () { ArticleId = 2, Name = "Article B", Price = 15.0m }
                };

                var articleDtos = new List<ArticleDto>
                {
                    new () { ArticleId = 1, Name = "Article A", Price = 10.0m },
                    new () { ArticleId = 2, Name = "Article B", Price = 15.0m }
                };



                // 3. Configure mock repository to return the test data
                mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(article);

                // 4. Configure mock mapper to return the mapped DTOs
                mockMapper.Setup(mapper => mapper.Map<IEnumerable<ArticleDto>>(article)).Returns(articleDtos);

                // 5. Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                // 6. Call the method under test
                var result = await service.GetAllArticlesAsync();

                // Assert
                // 7. Verify the result is not null
                Assert.NotNull(result);

                // 8. Verify the result contains the expected number of items
                Assert.Equal(2, result.Count());

                // 9. Verify the mock repository was called once
                mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);

                // 10. Verify the mock mapper was called once
                mockMapper.Verify(mapper => mapper.Map<IEnumerable<ArticleDto>>(article), Times.Once);
            }


            [Fact]
            // Scenario: The repository returns an empty list of orders.
            // Purpose: Ensure the service handles empty results correctly.
            public async Task GetArticleAsync_ShouldReturnEmptyList()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Set up mock data (empty list)
                var article = new List<Article>();
                var articleDtos = new List<ArticleDto>();

                // Configure mock repository to return the empty list
                mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(article);

                // Configure mock mapper to return the empty list of DTOs
                mockMapper.Setup(mapper => mapper.Map<IEnumerable<ArticleDto>>(article)).Returns(articleDtos);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                var result = await service.GetAllArticlesAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
                mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
                mockMapper.Verify(mapper => mapper.Map<IEnumerable<ArticleDto>>(articleDtos), Times.Once);
            }

            [Fact]
            //Scenario: The repository returns null.
            //Purpose: Ensure the service handles null results gracefully.
            public async Task GetAllArticleAsync_ShouldHandleNullResponse()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Configure mock repository to return null
                mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync((IEnumerable<Article>)null);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                var result = await service.GetAllArticlesAsync();

                // Assert
                Assert.Null(result);
                mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
                // Verify the mapper was never called
                mockMapper.Verify(mapper => mapper.Map<IEnumerable<ArticleDto>>(It.IsAny<List<Article>>()), Times.Never);
            }

            [Fact]
            //Scenario: The repository throws an exception.
            //Purpose: Ensure the service handles exceptions gracefully.
            public async Task GetAllArticleAsync_ShouldHandleRepositoryException()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Configure mock repository to throw an exception
                mockRepo.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => service.GetAllArticlesAsync());
                mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
                mockMapper.Verify(mapper => mapper.Map<IEnumerable<ArticleDto>>(It.IsAny<IEnumerable<Article>>()), Times.Never);
            }


            // ---> GetArticleByIdAsync Tests <---
            //Scenarios to Test
            //Happy Path: Returns an ArticleDto for a valid order line ID.   
            //Null Response: Handles null when the order is not found.
            //Exception Handling: Handles exceptions thrown by the repository.

            // Scenario: The repository returns a valid Order.
            [Fact]
            // Scenario: The repository returns a valid Order.
            public async Task GetArticleByIdAsync_ShouldReturnArticleDto()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Set up mock data
                var article = new Article
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };
                var articleDto = new ArticleDto
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };

                // Configure mock repository to return the test data
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(article);

                // Configure mock mapper to return the mapped DTO
                mockMapper.Setup(mapper => mapper.Map<ArticleDto>(article)).Returns(articleDto);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                var result = await service.GetArticleByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Article A", result.Name);
                Assert.Equal(10.0m, result.Price);
                mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
                mockMapper.Verify(mapper => mapper.Map<ArticleDto>(article), Times.Once);
            }

            // Scenario: The repository returns null (order not found).
            [Fact]
            public async Task GeArticleByIdAsync_ShouldHandleNullResponse()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Configure mock repository to return null
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Article)null);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                var result = await service.GetArticleByIdAsync(1);

                // Assert
                Assert.Null(result);
                mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
                mockMapper.Verify(mapper => mapper.Map<ArticleDto>(It.IsAny<Article>()), Times.Never);
            }

            // Scenario: The repository throws an exception.
            [Fact]
            public async Task GetArticleByIdAsync_ShouldHandleRepositoryException()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Configure mock repository to throw an exception
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => service.GetArticleByIdAsync(1));
                mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
                mockMapper.Verify(mapper => mapper.Map<ArticleDto>(It.IsAny<Article>()), Times.Never);
            }

            // ---> AddOrderAsync Tests <---
            //Scenarios to Test
            //Happy Path: The order is successfully created.
            //Null Input: The input OrderDto is null.
            //Exception Handling: The repository throws an exception.

            // Scenario: The order is successfully created.
            [Fact]
            public async Task ArticleAsync_ShouldCreateOrderLine()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Set up mock data
                var articleDto = new ArticleDto
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };
                var article = new Article
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };

                // Configure mock mapper to return the mapped entity
                mockMapper.Setup(mapper => mapper.Map<Article>(articleDto)).Returns(article);
                //mockMapper.Setup(mapper => mapper.Map<OrderLineDto>(orderLine)).Returns(orderLineDto);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                await service.CreateArticleAsync(articleDto);

                // Assert
                mockRepo.Verify(repo => repo.CreateAsync(article), Times.Once);
                mockMapper.Verify(mapper => mapper.Map<Article>(articleDto), Times.Once);
            }

            // Scenario: The input OrderDto is null.
            [Fact]
            public async Task AddArticleAsync_ShouldThrowExceptionWhenArticleDtoIsNull()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act & Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateArticleAsync(null));

                // Verify the repository and mapper were never called
                mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Article>()), Times.Never);
                mockMapper.Verify(mapper => mapper.Map<Article>(It.IsAny<ArticleDto>()), Times.Never);
            }


            // Scenario: The repository throws an exception.
            [Fact]
            public async Task AddArticleAsync_ShouldHandleRepositoryException()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Set up mock data
                var articleDto = new ArticleDto
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };
                var article = new Article
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };

                // Configure mock mapper to return the mapped entity
                mockMapper.Setup(mapper => mapper.Map<Article>(articleDto)).Returns(article);

                // Configure mock repository to throw an exception
                mockRepo.Setup(repo => repo.CreateAsync(article)).ThrowsAsync(new Exception("Database error"));

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => service.CreateArticleAsync(articleDto));
                mockRepo.Verify(repo => repo.CreateAsync(article), Times.Once);
                mockMapper.Verify(mapper => mapper.Map<Article>(articleDto), Times.Once);
            }


            // ---> UpdateArticleAsync Tests <---

            //Happy Path: The article is successfully updated.
            //Article Not Found: The repository returns null (ArticleLine not found).
            //Null Input: The input articledto is null.
            //Exception Handling: The repository throws an exception.

            //  Scenario: The article is successfully updated.
            [Fact]
            public async Task UpdateArticleAsync_ShouldUpdateArticle()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Set up mock data
                var existingArticle = new Article
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };

                var articleDto = new ArticleDto
                {
                    ArticleId = 1,
                    Name = "Updated Article A",
                    Price = 15.0m
                };

                // Configure mock repository to return the existing article
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingArticle);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                await service.UpdateArticleAsync(articleDto);

                // Assert
                mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
                mockRepo.Verify(repo => repo.UpdateAsync(existingArticle), Times.Once);
            }

            // Scenario: The repository returns null (order not found).
            [Fact]
            public async Task UpdateArticleAsync_ShouldThrowExceptionWhenArticleNotFound()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Configure mock repository to return null
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Article)null);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => service.UpdateArticleAsync(new ArticleDto { ArticleId = 1 }));                //var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderLineAsync(1, new OrderLineDto()));

                // Verify the exception message
                Assert.Equal("Article not found", exception.Message);

                // Verify the repository and mapper were never called
                mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Article>()), Times.Never);
                mockMapper.Verify(mapper => mapper.Map(It.IsAny<ArticleDto>(), It.IsAny<Article>()), Times.Never);
            }

            // Scenario: The input ArticleDto is null.
            [Fact]
            public async Task UpdateArticleAsync_ShouldThrowExceptionWhenArticleDtoIsNull()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateArticleAsync(null));

                // Verify the exception message
                Assert.Contains("ArticleDto cannot be null.", exception.Message);

                // Verify the repository and mapper were never called
                mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Article>()), Times.Never);
                mockMapper.Verify(mapper => mapper.Map(It.IsAny<ArticleDto>(), It.IsAny<Article>()), Times.Never);

            }

            // ---> DeleteArticleAsync Tests <---
            //Happy Path: The Article is successfully deleted.
            //Article Not Found: The repository returns null (Article not found).
            //Exception Handling: The repository throws an exception.

            // Scenario: The order is successfully deleted.
            [Fact]
            public async Task DeleteArticleAsync_ShouldDeleteArticle()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();


                // Set up mock data
                var existingArticle = new Article
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };

                // Configure mock repository to return the existing order
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingArticle);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act
                await service.DeleteArticleAsync(1);

                // Assert
                mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
                mockRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
            }

            // Scenario: The repository returns null (article not found).
            [Fact]
            public async Task DeleteArticleAsync_ShouldReturnFalseWhenArticleNotFound()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();


                // Configure mock repository to return null
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Article)null);

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);


                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => service.DeleteArticleAsync(1));

                // Verify the exception message
                Assert.Equal("Article not found", exception.Message);

                // Verify the repository was never called
                mockRepo.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);  
            }


            // Scenario: The repository throws an exception.
            [Fact]
            public async Task DeleteArticleAsync_ShouldHandleRepositoryException()
            {
                // Arrange
                var mockRepo = new Mock<IArticleRepository>();
                var mockMapper = new Mock<IMapper>();

                // Set up mock data
                var existingArticle = new Article
                {
                    ArticleId = 1,
                    Name = "Article A",
                    Price = 10.0m
                };

                // Configure mock repository to return the existing order
                mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingArticle);

                // Configure mock repository to throw an exception
                mockRepo.Setup(repo => repo.DeleteAsync(1)).ThrowsAsync(new Exception("Database error"));

                // Create an instance of the service with the mocked dependencies
                var service = new ArticleService(mockRepo.Object, mockMapper.Object);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => service.DeleteArticleAsync(1));

                // Verify the repository was called
                mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
                mockRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
            }

        }
    }
}

