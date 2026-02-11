using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("🚀 LinkedIn AutoPoster — Starting up...");
        Console.WriteLine($"📅 {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        Console.WriteLine();

        // Build configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Read LinkedIn settings — environment variables take priority over appsettings.json
        var accessToken = Environment.GetEnvironmentVariable("LINKEDIN_TOKEN") 
                        ?? config["LinkedIn:AccessToken"];
        var authorUrn = Environment.GetEnvironmentVariable("LINKEDIN_AUTHOR_URN") 
                        ?? config["LinkedIn:AuthorUrn"];
        var visibility = Environment.GetEnvironmentVariable("VISIBILITY") 
                        ?? config["LinkedIn:Visibility"] 
                        ?? "PUBLIC";

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(authorUrn))
        {
            Console.WriteLine("❌ Missing configuration. Please set up appsettings.json or environment variables.");
            Console.WriteLine();
            Console.WriteLine("Required settings:");
            Console.WriteLine("  • LINKEDIN_TOKEN or LinkedIn:AccessToken");
            Console.WriteLine("  • LINKEDIN_AUTHOR_URN or LinkedIn:AuthorUrn");
            Console.WriteLine();
            Console.WriteLine("Optional settings:");
            Console.WriteLine("  • VISIBILITY or LinkedIn:Visibility (CONNECTIONS or PUBLIC, default: CONNECTIONS)");
            return;
        }

        Console.WriteLine("✅ Configuration loaded successfully");
        Console.WriteLine($"👤 Author: {authorUrn}");
        Console.WriteLine($"👁️  Visibility: {visibility}");
        Console.WriteLine();

        // Fetch news from curated RSS feeds
        Console.WriteLine("📡 Fetching latest tech news...");
        var fetcher = new NewsFetcher();
        var newsItem = fetcher.GetLatestNews();

        if (newsItem == null)
        {
            Console.WriteLine("❌ Could not fetch any news. All feeds may be down. Exiting.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"📰 Selected: [{newsItem.Category}] {newsItem.Title}");
        Console.WriteLine();

        // Post to LinkedIn
        Console.WriteLine("📤 Posting to LinkedIn...");
        var poster = new LinkedInPoster(accessToken, authorUrn, visibility);
        poster.Post(newsItem);

        Console.WriteLine();
        Console.WriteLine("🎉 Done! See you tomorrow.");
    }
}