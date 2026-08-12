using ECommerce.Core.Models;

namespace ECommerce.Core.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<List<Category>> GetCategoriesAsync();
}
