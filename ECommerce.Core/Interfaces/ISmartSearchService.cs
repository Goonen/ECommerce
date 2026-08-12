using ECommerce.Core.Models;

namespace ECommerce.Core.Interfaces;

public interface ISmartSearchService
{
    /// <summary>
    /// Finds products matching a free-text description, in any language.
    /// </summary>
    Task<SmartSearchResult> SearchByTextAsync(string query, int count = 8);

    /// <summary>
    /// Finds products matching an uploaded photo of a desired item.
    /// </summary>
    Task<SmartSearchResult> SearchByImageAsync(byte[] imageBytes, string mediaType, int count = 8);
}
