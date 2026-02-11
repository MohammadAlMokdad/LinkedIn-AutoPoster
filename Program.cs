using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        // Build configuration - check environment variables first, then JSON file
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Read settings - try environment variables first, then appsettings.json
        var accessToken = Environment.GetEnvironmentVariable("LINKEDIN_TOKEN") 
                        ?? config["LinkedIn:AccessToken"];
        var authorUrn = Environment.GetEnvironmentVariable("LINKEDIN_AUTHOR_URN") 
                        ?? config["LinkedIn:AuthorUrn"];
        var visibility = Environment.GetEnvironmentVariable("VISIBILITY") 
                        ?? config["LinkedIn:Visibility"] 
                        ?? "CONNECTIONS";
        var rssFeed = Environment.GetEnvironmentVariable("RSS_FEED_URL") 
                        ?? config["RssFeedUrl"];

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(authorUrn) || string.IsNullOrEmpty(rssFeed))
        {
            Console.WriteLine("Missing configuration. Please set up appsettings.json or environment variables.");
            Console.WriteLine("\nRequired settings:");
            Console.WriteLine("  - LinkedIn:AccessToken (Your LinkedIn API access token)");
            Console.WriteLine("  - LinkedIn:AuthorUrn (Your LinkedIn author URN, e.g., urn:li:person:XXXX)");
            Console.WriteLine("  - RssFeedUrl (RSS feed URL to fetch news from)");
            Console.WriteLine("\nOptional settings:");
            Console.WriteLine("  - LinkedIn:Visibility (CONNECTIONS or PUBLIC, default: CONNECTIONS)");
            return;
        }

        var fetcher = new NewsFetcher(rssFeed);
        var poster = new LinkedInPoster(accessToken, authorUrn, visibility);

        var newsText = fetcher.GetLatestNews();
        Console.WriteLine($"Post content: {newsText}");

        poster.Post(newsText);
    }
}