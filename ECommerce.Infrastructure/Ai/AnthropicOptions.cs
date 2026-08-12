namespace ECommerce.Infrastructure.Ai;

public class AnthropicOptions
{
    /// <summary>
    /// Your Anthropic API key. Prefer setting this via the ANTHROPIC_API_KEY environment
    /// variable or user secrets rather than committing it to appsettings.json.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Model used for interpreting search queries/images. A small, fast model is enough
    /// for this classification-style task.
    /// </summary>
    public string Model { get; set; } = "claude-sonnet-5";
}
