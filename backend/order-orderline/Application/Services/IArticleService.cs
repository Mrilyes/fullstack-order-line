using order_orderline.Application.DTOs;
using order_orderline.Domain.Entites;

namespace order_orderline.Application.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
        Task<ArticleDto> GetArticleByIdAsync(int id);
        Task CreateArticleAsync(ArticleDto articleDto);
        Task UpdateArticleAsync(ArticleDto articleDto);
        Task DeleteArticleAsync(int id);
    }
}
