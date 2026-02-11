using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

/// <summary>
/// Posts beautifully formatted tech news to LinkedIn using the Posts API.
/// </summary>
public class LinkedInPoster
{
    private readonly string _accessToken;
    private readonly string _authorUrn;
    private readonly string _visibility;

    // Unicode bold text mapping for eye-catching headers (surrogate pairs, so we use strings)
    private static readonly Dictionary<char, string> BoldMap = new()
    {
        ['A'] = "\U0001D5D4", ['B'] = "\U0001D5D5", ['C'] = "\U0001D5D6", ['D'] = "\U0001D5D7",
        ['E'] = "\U0001D5D8", ['F'] = "\U0001D5D9", ['G'] = "\U0001D5DA", ['H'] = "\U0001D5DB",
        ['I'] = "\U0001D5DC", ['J'] = "\U0001D5DD", ['K'] = "\U0001D5DE", ['L'] = "\U0001D5DF",
        ['M'] = "\U0001D5E0", ['N'] = "\U0001D5E1", ['O'] = "\U0001D5E2", ['P'] = "\U0001D5E3",
        ['Q'] = "\U0001D5E4", ['R'] = "\U0001D5E5", ['S'] = "\U0001D5E6", ['T'] = "\U0001D5E7",
        ['U'] = "\U0001D5E8", ['V'] = "\U0001D5E9", ['W'] = "\U0001D5EA", ['X'] = "\U0001D5EB",
        ['Y'] = "\U0001D5EC", ['Z'] = "\U0001D5ED",
        ['a'] = "\U0001D5EE", ['b'] = "\U0001D5EF", ['c'] = "\U0001D5F0", ['d'] = "\U0001D5F1",
        ['e'] = "\U0001D5F2", ['f'] = "\U0001D5F3", ['g'] = "\U0001D5F4", ['h'] = "\U0001D5F5",
        ['i'] = "\U0001D5F6", ['j'] = "\U0001D5F7", ['k'] = "\U0001D5F8", ['l'] = "\U0001D5F9",
        ['m'] = "\U0001D5FA", ['n'] = "\U0001D5FB", ['o'] = "\U0001D5FC", ['p'] = "\U0001D5FD",
        ['q'] = "\U0001D5FE", ['r'] = "\U0001D5FF", ['s'] = "\U0001D600", ['t'] = "\U0001D601",
        ['u'] = "\U0001D602", ['v'] = "\U0001D603", ['w'] = "\U0001D604", ['x'] = "\U0001D605",
        ['y'] = "\U0001D606", ['z'] = "\U0001D607",
        ['0'] = "\U0001D7EC", ['1'] = "\U0001D7ED", ['2'] = "\U0001D7EE", ['3'] = "\U0001D7EF",
        ['4'] = "\U0001D7F0", ['5'] = "\U0001D7F1", ['6'] = "\U0001D7F2", ['7'] = "\U0001D7F3",
        ['8'] = "\U0001D7F4", ['9'] = "\U0001D7F5",
    };

    public LinkedInPoster(string accessToken, string authorUrn, string visibility)
    {
        _accessToken = accessToken;
        _authorUrn = authorUrn;
        _visibility = visibility;
    }

    /// <summary>
    /// Creates a visually stunning LinkedIn post from a NewsItem.
    /// </summary>
    public void Post(NewsItem news)
    {
        var postText = FormatPost(news);
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("📋 Post Preview:");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine(postText);
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"📏 Character count: {postText.Length}/3000");
        Console.WriteLine();

        PublishToLinkedIn(postText);
    }

    /// <summary>
    /// Formats the news article into a beautiful, engaging LinkedIn post.
    /// </summary>
    public static string FormatPost(NewsItem news)
    {
        var title = ToBold("Daily Tech Insight");
        var sb = new StringBuilder();

        // Header with visual impact
        sb.AppendLine($"\U0001F525 {title} \u2014 {news.CategoryEmoji} {news.Category}");
        sb.AppendLine();
        sb.AppendLine("\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501");
        sb.AppendLine();

        // Article title
        sb.AppendLine($"\U0001F4F0 {news.Title}");
        sb.AppendLine();

        // Description with clean formatting
        if (!string.IsNullOrWhiteSpace(news.Description))
        {
            sb.AppendLine(news.Description);
            sb.AppendLine();
        }

        // Link with call to action
        if (!string.IsNullOrWhiteSpace(news.Link))
        {
            sb.AppendLine($"\U0001F517 Read the full article: {news.Link}");
            sb.AppendLine();
        }

        sb.AppendLine("\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501");
        sb.AppendLine();

        // Engagement footer
        sb.AppendLine("\U0001F4A1 Stay ahead of the curve \u2014 Follow for daily tech insights!");
        sb.AppendLine("\u267B\uFE0F  Found this useful? Repost to help your network!");
        sb.AppendLine();

        // Strategic hashtags for reach
        sb.Append(GetHashtags(news.Category));

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Returns relevant hashtags based on the article category.
    /// </summary>
    private static string GetHashtags(string category)
    {
        var baseHashtags = "#TechNews #Innovation #DailyInsight";

        var categoryHashtags = category.ToLowerInvariant() switch
        {
            var c when c.Contains("cyber") || c.Contains("security") || c.Contains("hack")
                => "#CyberSecurity #InfoSec #Hacking #Privacy #ThreatIntelligence",
            var c when c.Contains("ai") || c.Contains("machine") || c.Contains("deep")
                => "#AI #MachineLearning #DeepLearning #ArtificialIntelligence #GenerativeAI",
            var c when c.Contains("dev") || c.Contains("tool") || c.Contains("github")
                => "#DevTools #OpenSource #GitHub #SoftwareDevelopment #Programming",
            var c when c.Contains("research")
                => "#AIResearch #ComputerScience #Technology #Research #Science",
            _   => "#Technology #Software #Engineering #Programming #Tech"
        };

        return $"{baseHashtags}\n{categoryHashtags}";
    }

    /// <summary>
    /// Converts ASCII text to Unicode Mathematical Bold Sans-Serif characters for LinkedIn.
    /// </summary>
    private static string ToBold(string text)
    {
        var sb = new StringBuilder();
        foreach (var c in text)
        {
            if (BoldMap.TryGetValue(c, out var bold))
                sb.Append(bold);
            else
                sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Publishes the formatted post to LinkedIn using the new Posts API.
    /// </summary>
    private void PublishToLinkedIn(string postText)
    {
        try
        {
            // Build the JSON payload using the new Posts API schema
            var payload = new
            {
                author = _authorUrn,
                commentary = postText,
                visibility = _visibility,
                distribution = new
                {
                    feedDistribution = "MAIN_FEED",
                    targetEntities = Array.Empty<object>(),
                    thirdPartyDistributionChannels = Array.Empty<object>()
                },
                lifecycleState = "PUBLISHED",
                isReshareDisabledByAuthor = false
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
            client.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");
            client.DefaultRequestHeaders.Add("LinkedIn-Version", "202401");

            var response = client.PostAsync(
                "https://api.linkedin.com/rest/posts",
                new StringContent(json, Encoding.UTF8, "application/json")
            ).Result;

            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("\u2705 LinkedIn post published successfully!");

                // The Posts API returns the post URN in the x-restli-id header
                if (response.Headers.TryGetValues("x-restli-id", out var values))
                {
                    Console.WriteLine($"\U0001F4CC Post ID: {values.First()}");
                }
            }
            else
            {
                Console.WriteLine($"\u274C LinkedIn post failed: {response.StatusCode}");
                Console.WriteLine($"\U0001F4C4 Error details: {responseContent}");

                // If the new API fails, attempt fallback to legacy API
                Console.WriteLine("\n\U0001F504 Attempting fallback to legacy UGC API...");
                PublishToLinkedInLegacy(postText);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\u274C Error posting to LinkedIn: {ex.Message}");
        }
    }

    /// <summary>
    /// Fallback: Posts using the legacy v2/ugcPosts API in case the new API is not yet available.
    /// </summary>
    private void PublishToLinkedInLegacy(string postText)
    {
        try
        {
            var payload = new
            {
                author = _authorUrn,
                lifecycleState = "PUBLISHED",
                specificContent = new Dictionary<string, object>
                {
                    ["com.linkedin.ugc.ShareContent"] = new
                    {
                        shareCommentary = new { text = postText },
                        shareMediaCategory = "NONE"
                    }
                },
                visibility = new Dictionary<string, string>
                {
                    ["com.linkedin.ugc.MemberNetworkVisibility"] = _visibility
                }
            };

            var json = JsonSerializer.Serialize(payload);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
            client.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");

            var response = client.PostAsync(
                "https://api.linkedin.com/v2/ugcPosts",
                new StringContent(json, Encoding.UTF8, "application/json")
            ).Result;

            var responseContent = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("\u2705 LinkedIn post published via legacy API!");
                var shareId = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (shareId.TryGetProperty("id", out var id))
                    Console.WriteLine($"\U0001F4CC Share ID: {id.GetString()}");
            }
            else
            {
                Console.WriteLine($"\u274C Legacy API also failed: {response.StatusCode}");
                Console.WriteLine($"\U0001F4C4 Error details: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\u274C Legacy fallback error: {ex.Message}");
        }
    }
}