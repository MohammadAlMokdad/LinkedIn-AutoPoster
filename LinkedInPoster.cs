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
            var postContent = new
            {
                author = authorUrn,
                lifecycleState = "PUBLISHED",
                specificContent = new
                {
                    comLinkedinUgcShareContent = new
                    {
                        shareCommentary = new { text },
                        shareMediaCategory = "NONE"
                    }
                },
                visibility = new { comLinkedinUgcMemberNetworkVisibility = visibility }
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var json = JsonSerializer.Serialize(postContent);
            var response = client.PostAsync(
                "https://api.linkedin.com/v2/ugcPosts",
                new StringContent(json, Encoding.UTF8, "application/json")
            ).Result;

            if (response.IsSuccessStatusCode)
                Console.WriteLine("✅ LinkedIn post successful!");
            else
                Console.WriteLine($"❌ LinkedIn post failed: {response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error posting to LinkedIn: {ex.Message}");
        }
    }
}
