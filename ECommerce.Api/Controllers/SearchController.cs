using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISmartSearchService _smartSearch;

    public SearchController(ISmartSearchService smartSearch)
    {
        _smartSearch = smartSearch;
    }

    public class TextSearchRequest
    {
        public string Query { get; set; } = string.Empty;
    }

    /// <summary>Search using a free-text description, in any language.</summary>
    [HttpPost("text")]
    public async Task<IActionResult> SearchByText([FromBody] TextSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Query is required.");

        try
        {
            var result = await _smartSearch.SearchByTextAsync(request.Query);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    /// <summary>Search using an uploaded photo of the desired item (multipart/form-data, field name "image").</summary>
    [HttpPost("image")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> SearchByImage(IFormFile image)
    {
        if (image is null || image.Length == 0)
            return BadRequest("An image file is required.");

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);

        try
        {
            var result = await _smartSearch.SearchByImageAsync(ms.ToArray(), image.ContentType);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
