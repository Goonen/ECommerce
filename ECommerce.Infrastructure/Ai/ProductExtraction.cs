namespace ECommerce.Infrastructure.Ai;

/// <summary>
/// What the AI understood about a customer's text description or uploaded image,
/// mapped onto this store's existing category/tag taxonomy so it can be scored
/// against the catalog the same way product-to-product recommendations are.
/// </summary>
public class ProductExtraction
{
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string? Summary { get; set; }
}
