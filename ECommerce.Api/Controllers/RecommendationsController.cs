using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationsController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetRecommendations(int productId, [FromQuery] int count = 4)
    {
        var recommendations = await _recommendationService.GetRecommendationsAsync(productId, count);
        return Ok(recommendations);
    }
}
