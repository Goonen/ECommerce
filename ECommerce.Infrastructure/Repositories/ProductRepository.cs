using ECommerce.Core.Interfaces;
using ECommerce.Core.Models;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync() =>
        await _context.Products.Include(p => p.Category).ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Category>> GetCategoriesAsync() =>
        await _context.Categories.ToListAsync();
}
