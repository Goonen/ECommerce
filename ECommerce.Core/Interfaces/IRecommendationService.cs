using ECommerce.Core.Models;

namespace ECommerce.Core.Interfaces;

public interface IRecommendationService
{
    /// <summary>
    /// Returns up to <paramref name="count"/> products related to the given product,
    /// ranked by a similarity score (shared category, shared tags, price proximity).
    /// </summary>
    Task<List<Product>> GetRecommendationsAsync(int productId, int count = 4);
}
