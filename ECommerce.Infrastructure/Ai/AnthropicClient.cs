using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Ai;

/// <summary>
/// Minimal wrapper around POST /v1/messages. Only what this app needs:
/// send a system prompt plus optional text/image content, get back the text reply.
/// </summary>
public class AnthropicClient
{
    private readonly HttpClient _http;
    private readonly AnthropicOptions _options;

    public AnthropicClient(HttpClient http, IOptions<AnthropicOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey);

    public async Task<string> AskAsync(
        string systemPrompt,
        string userText,
        byte[]? imageBytes = null,
        string? imageMediaType = null)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(
                "Anthropic API key is not configured. Set Anthropic:ApiKey in appsettings " +
                "(or the ANTHROPIC_API_KEY environment variable) to enable AI-powered search.");
        }

        var contentBlocks = new List<object>();

        if (imageBytes is not null && !string.IsNullOrWhiteSpace(imageMediaType))
        {
            contentBlocks.Add(new
            {
                type = "image",
                source = new
                {
                    type = "base64",
                    media_type = imageMediaType,
                    data = Convert.ToBase64String(imageBytes)
                }
            });
        }

        contentBlocks.Add(new { type = "text", text = userText });

        var payload = new
        {
            model = _options.Model,
            max_tokens = 500,
            system = systemPrompt,
            messages = new[]
            {
                new { role = "user", content = contentBlocks }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/messages")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        using var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Anthropic API error ({(int)response.StatusCode}): {body}");
        }

        using var doc = JsonDocument.Parse(body);
        foreach (var block in doc.RootElement.GetProperty("content").EnumerateArray())
        {
            if (block.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "text")
            {
                return block.GetProperty("text").GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }
}
