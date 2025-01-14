using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using order_orderline.Application.DTOs;
using order_orderline.Domain.Entites;
using order_orderline.Infrastructure.Repositories;

namespace order_orderline.Application.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _repository;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
        {
            var articles = await _repository.GetAllAsync();
            if (articles == null)
            {
                return null;
            }

            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public async Task<ArticleDto> GetArticleByIdAsync(int id)
        {
            var article = await _repository.GetByIdAsync(id);
            if (article == null)
            {
                return null;
            }
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task CreateArticleAsync(ArticleDto articleDto)
        {
            // Check for null input
            if (articleDto == null)
            {
                throw new ArgumentNullException(nameof(articleDto), "OrderLineDto cannot be null.");
            }

            var article = _mapper.Map<Article>(articleDto);
            await _repository.CreateAsync(article);
        }

        public async Task UpdateArticleAsync(ArticleDto articleDto)
        {
            // Check for null input
            if (articleDto == null)
            {
                throw new ArgumentNullException(nameof(articleDto), "ArticleDto cannot be null.");
            }

            // Fetch the existing article from the repository
            var existingArticle = await _repository.GetByIdAsync(articleDto.ArticleId);
            if (existingArticle == null)
            {
                throw new Exception("Article not found");
            }

            // Map the updated DTO to the existing article entity
            _mapper.Map(articleDto, existingArticle);
            // Perform the update in the repository
            await _repository.UpdateAsync(existingArticle);
        }

        public async Task DeleteArticleAsync(int id)
        {
            // Fetch the existing article from the repository
            var existingArticle = await _repository.GetByIdAsync(id);
            if (existingArticle == null)
            {
                throw new Exception("Article not found");
            }
            await _repository.DeleteAsync(id);
        }
    }
}
