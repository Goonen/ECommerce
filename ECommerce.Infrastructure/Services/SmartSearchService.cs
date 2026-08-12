using System.Text.Json;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Models;
using ECommerce.Infrastructure.Ai;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

/// <summary>
/// Lets a customer find products by describing what they want in their own language,
/// or by uploading a photo. Claude maps the request onto this store's existing
/// category/tag taxonomy, and the result is scored against the catalog using the
/// same explainable, rule-based approach as <see cref="RuleBasedRecommendationService"/>:
///   +3  category matches
///   +2  per matching tag
///   +1  per keyword found in the product name/description
/// If nothing scores above 0 (e.g. the AI's wording didn't line up with the catalog),
/// falls back to a loose substring match so the customer still gets a result.
/// </summary>
public class SmartSearchService : ISmartSearchService
{
    private const int CategoryWeight = 3;
    private const int TagWeight = 2;
    private const int KeywordWeight = 1;

    private readonly AppDbContext _context;
    private readonly AnthropicClient _ai;

    public SmartSearchService(AppDbContext context, AnthropicClient ai)
    {
        _context = context;
        _ai = ai;
    }

    public async Task<SmartSearchResult> SearchByTextAsync(string query, int count = 8)
    {
        var extraction = await ExtractAsync(query, null, null);
        return await BuildResultAsync(extraction, count);
    }

    public async Task<SmartSearchResult> SearchByImageAsync(byte[] imageBytes, string mediaType, int count = 8)
    {
        var extraction = await ExtractAsync(
            "The customer uploaded this photo of something they want to buy. Identify it.",
            imageBytes,
            mediaType);
        return await BuildResultAsync(extraction, count);
    }

    private async Task<ProductExtraction> ExtractAsync(string userText, byte[]? imageBytes, string? mediaType)
    {
        var categories = await _context.Categories.Select(c => c.Name).ToListAsync();
        var allTags = await _context.Products.Select(p => p.Tags).ToListAsync();
        var distinctTags = allTags
            .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Select(t => t.ToLowerInvariant())
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        var systemPrompt =
            "You are a product-search assistant for an online store. The customer may write in ANY " +
            "language, or send a photo instead of text. Map their request onto this store's catalog.\n\n" +
            $"Available categories: {string.Join(", ", categories)}\n" +
            $"Available tags: {string.Join(", ", distinctTags)}\n\n" +
            "Reply with ONLY a JSON object — no markdown code fences, no commentary — in exactly this shape:\n" +
            "{\"category\": \"<one of the available categories above, or null if none fit>\", " +
            "\"tags\": [\"<subset of the available tags above that apply>\"], " +
            "\"keywords\": [\"<a few short English search keywords describing the item>\"], " +
            "\"summary\": \"<one short sentence, in English, describing what the customer wants>\"}";

        var raw = await _ai.AskAsync(systemPrompt, userText, imageBytes, mediaType);
        return ParseExtraction(raw);
    }

    private static ProductExtraction ParseExtraction(string raw)
    {
        var cleaned = raw.Trim();

        // Defensive: strip ```json ... ``` fences if the model adds them despite instructions.
        if (cleaned.StartsWith("```"))
        {
            cleaned = cleaned.Trim('`');
            var newlineIndex = cleaned.IndexOf('\n');
            if (newlineIndex >= 0) cleaned = cleaned[(newlineIndex + 1)..];
        }

        try
        {
            var extraction = JsonSerializer.Deserialize<ProductExtraction>(
                cleaned,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return extraction ?? new ProductExtraction();
        }
        catch (JsonException)
        {
            // Model didn't return clean JSON — fall back to treating the raw reply as a keyword
            // so the search can still attempt a loose text match instead of failing outright.
            return new ProductExtraction { Keywords = new List<string> { cleaned }, Summary = cleaned };
        }
    }

    private async Task<SmartSearchResult> BuildResultAsync(ProductExtraction extraction, int count)
    {
        var products = await _context.Products.Include(p => p.Category).ToListAsync();

        var scored = products
            .Select(p => new { Product = p, Score = Score(extraction, p) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Product.Name)
            .Take(count)
            .Select(x => x.Product)
            .ToList();

        if (scored.Count == 0)
        {
            var needles = extraction.Keywords
                .Append(extraction.Summary ?? string.Empty)
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Select(k => k.ToLowerInvariant())
                .ToList();

            scored = products
                .Where(p => needles.Any(n =>
                    p.Name.ToLowerInvariant().Contains(n) ||
                    p.Description.ToLowerInvariant().Contains(n) ||
                    p.Tags.ToLowerInvariant().Contains(n)))
                .Take(count)
                .ToList();
        }

        return new SmartSearchResult
        {
            Products = scored,
            InterpretedAs = extraction.Summary
        };
    }

    private static int Score(ProductExtraction extraction, Product candidate)
    {
        var score = 0;

        if (!string.IsNullOrWhiteSpace(extraction.Category) &&
            string.Equals(candidate.Category?.Name, extraction.Category, StringComparison.OrdinalIgnoreCase))
        {
            score += CategoryWeight;
        }

        var candidateTags = candidate.TagList.Select(t => t.ToLowerInvariant()).ToHashSet();
        score += candidateTags.Intersect(extraction.Tags.Select(t => t.ToLowerInvariant())).Count() * TagWeight;

        var haystack = $"{candidate.Name} {candidate.Description}".ToLowerInvariant();
        score += extraction.Keywords.Count(k =>
            !string.IsNullOrWhiteSpace(k) && haystack.Contains(k.ToLowerInvariant())) * KeywordWeight;

        return score;
    }
}
