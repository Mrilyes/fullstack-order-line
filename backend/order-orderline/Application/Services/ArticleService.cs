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
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public async Task<ArticleDto> GetArticleByIdAsync(int id)
        {
            var article = await _repository.GetByIdAsync(id);
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task CreateArticleAsync(ArticleDto articleDto)
        {
            var article = _mapper.Map<Article>(articleDto);
            await _repository.CreateAsync(article);
        }

        public async Task UpdateArticleAsync(ArticleDto articleDto)
        {
            var article = _mapper.Map<Article>(articleDto);
            await _repository.UpdateAsync(article);
        }

        public async Task DeleteArticleAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
