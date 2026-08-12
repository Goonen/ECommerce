using ECommerce.Core.Interfaces;
using ECommerce.Core.Models;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Scores candidate products against a source product using simple, explainable rules:
///   +3  same category
///   +2  per shared tag
///   +1  price within 20% of the source product's price
/// Highest-scoring products win. No score above 0 is excluded (i.e. completely
/// unrelated products are never recommended, even if the top-N quota isn't filled).
/// </summary>
public class RuleBasedRecommendationService : IRecommendationService
{
    private const int SameCategoryWeight = 3;
    private const int SharedTagWeight = 2;
    private const int PriceProximityWeight = 1;
    private const double PriceProximityThreshold = 0.20; // 20%

    private readonly AppDbContext _context;

    public RuleBasedRecommendationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetRecommendationsAsync(int productId, int count = 4)
    {
        var source = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (source is null) return new List<Product>();

        var candidates = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id != productId)
            .ToListAsync();

        var sourceTags = source.TagList;

        var scored = candidates
            .Select(candidate => new
            {
                Product = candidate,
                Score = Score(source, sourceTags, candidate)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Product.Name)
            .Take(count)
            .Select(x => x.Product)
            .ToList();

        return scored;
    }

    private static int Score(Product source, List<string> sourceTags, Product candidate)
    {
        var score = 0;

        if (candidate.CategoryId == source.CategoryId)
            score += SameCategoryWeight;

        var sharedTags = candidate.TagList.Intersect(sourceTags, StringComparer.OrdinalIgnoreCase).Count();
        score += sharedTags * SharedTagWeight;

        if (source.Price > 0)
        {
            var priceDelta = Math.Abs(candidate.Price - source.Price) / source.Price;
            if (priceDelta <= (decimal)PriceProximityThreshold)
                score += PriceProximityWeight;
        }

        return score;
    }
}
