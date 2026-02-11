using System;

class Program
{
    static void Main()
    {
        // Read environment variables (for GitHub Actions)
        var accessToken = Environment.GetEnvironmentVariable("LINKEDIN_TOKEN");
        var authorUrn = Environment.GetEnvironmentVariable("LINKEDIN_AUTHOR_URN");
        var visibility = Environment.GetEnvironmentVariable("VISIBILITY") ?? "CONNECTIONS";
        var rssFeed = Environment.GetEnvironmentVariable("RSS_FEED_URL");

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(authorUrn) || string.IsNullOrEmpty(rssFeed))
        {
            Console.WriteLine("❌ Missing environment variables. Exiting.");
            return;
        }

        var fetcher = new NewsFetcher(rssFeed);
        var poster = new LinkedInPoster(accessToken, authorUrn, visibility);

        var newsText = fetcher.GetLatestNews();
        Console.WriteLine($"📄 Post content: {newsText}");

        poster.Post(newsText);
    }
}
