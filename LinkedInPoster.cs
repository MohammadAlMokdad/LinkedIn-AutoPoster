using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class LinkedInPoster
{
    private readonly string accessToken;
    private readonly string authorUrn;
    private readonly string visibility;

    public LinkedInPoster(string accessToken, string authorUrn, string visibility)
    {
        this.accessToken = accessToken;
        this.authorUrn = authorUrn;
        this.visibility = visibility;
    }

    public void Post(string text)
    {
        try
        {
            // Build JSON manually with correct property names for LinkedIn API
            var json = "{" +
              "\"author\": \"" + authorUrn + "\"," +
              "\"lifecycleState\": \"PUBLISHED\"," +
              "\"specificContent\": {" +
                "\"com.linkedin.ugc.ShareContent\": {" +
                  "\"shareCommentary\": {" +
                    "\"text\": \"" + text.Replace("\"", "\\\"") + "\"" +
                  "}," +
                  "\"shareMediaCategory\": \"NONE\"" +
                "}" +
              "}," +
              "\"visibility\": {" +
                "\"com.linkedin.ugc.MemberNetworkVisibility\": \"" + visibility + "\"" +
              "}" +
            "}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");

            var response = client.PostAsync(
                "https://api.linkedin.com/v2/ugcPosts",
                new StringContent(json, Encoding.UTF8, "application/json")
            ).Result;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("LinkedIn post successful!");
                var shareId = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (shareId.TryGetProperty("id", out var id))
                    Console.WriteLine($"Share ID: {id.GetString()}");
            }
            else
            {
                Console.WriteLine($"LinkedIn post failed: {response.StatusCode}");
                Console.WriteLine($"Error details: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error posting to LinkedIn: {ex.Message}");
        }
    }
}