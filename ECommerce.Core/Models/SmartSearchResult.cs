namespace ECommerce.Core.Models;

public class SmartSearchResult
{
    public List<Product> Products { get; set; } = new();

    /// <summary>
    /// Short, human-readable summary of what the AI understood the customer wants
    /// (e.g. "wireless running headphones"), shown back to the user for transparency.
    /// </summary>
    public string? InterpretedAs { get; set; }
}
