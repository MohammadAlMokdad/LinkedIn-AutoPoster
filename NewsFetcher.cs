using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml;

/// <summary>
/// Represents a single news article fetched from an RSS feed.
/// </summary>
public record NewsItem(string Title, string Link, string Description, string Category, string CategoryEmoji);

/// <summary>
/// Represents a configured RSS feed source.
/// </summary>
public record FeedSource(string Name, string Url, string Category, string CategoryEmoji);

/// <summary>
/// Fetches tech news from multiple curated RSS feeds targeted at
/// developers, AI engineers, and cybersecurity specialists.
/// </summary>
public class NewsFetcher
{
    private readonly List<FeedSource> _feeds;
    private readonly HttpClient _client;

    /// <summary>
    /// Default curated RSS feeds covering AI, cybersecurity, dev tools, and tech news.
    /// </summary>
    public static readonly List<FeedSource> DefaultFeeds = new()
    {
        new("The Hacker News",       "https://feeds.feedburner.com/TheHackersNews",                  "Cybersecurity & Hacking",  "🛡️"),
        new("BleepingComputer",      "https://www.bleepingcomputer.com/feed/",                       "Cybersecurity News",       "🔒"),
        new("Krebs on Security",     "https://krebsonsecurity.com/feed/",                            "Security Research",        "🔐"),
        new("GitHub Blog",           "https://github.blog/feed/",                                    "Developer Tools",          "💻"),
        new("MIT News – AI",         "https://news.mit.edu/topic/mitartificial-intelligence2-rss.xml","AI Research",              "🔬"),
        new("Ars Technica",          "https://feeds.arstechnica.com/arstechnica/index",              "Tech News",                "🚀"),
        new("Google AI Blog",        "https://blog.google/technology/ai/rss/",                       "AI & Machine Learning",    "🤖"),
    };

    public NewsFetcher() : this(DefaultFeeds) { }

    public NewsFetcher(List<FeedSource> feeds)
    {
        _feeds = feeds;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "LinkedInAutoPoster/1.0");
        _client.Timeout = TimeSpan.FromSeconds(15);
    }

    /// <summary>
    /// Fetches the latest news by trying feeds in random order until one succeeds.
    /// Uses the current day-of-year to rotate through categories for variety.
    /// </summary>
    public NewsItem? GetLatestNews()
    {
        // Shuffle feeds using day-based seed for daily variety, but deterministic within a day
        var today = DateTime.UtcNow.DayOfYear + DateTime.UtcNow.Year * 1000;
        var shuffled = _feeds.OrderBy(f => HashCode.Combine(f.Name, today)).ToList();

        foreach (var feed in shuffled)
        {
            try
            {
                Console.WriteLine($"  📡 Trying feed: {feed.Name} ({feed.Category})...");
                var item = FetchFromFeed(feed);
                if (item != null)
                {
                    Console.WriteLine($"  ✅ Got article from {feed.Name}");
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠️  Feed {feed.Name} failed: {ex.Message}");
            }
        }

        Console.WriteLine("  ❌ All feeds failed.");
        return null;
    }

    private NewsItem? FetchFromFeed(FeedSource feed)
    {
        var xml = _client.GetStringAsync(feed.Url).Result;

        var doc = new XmlDocument();
        doc.LoadXml(xml);

        // Handle both RSS <item> and Atom <entry> formats
        var nsMgr = new XmlNamespaceManager(doc.NameTable);
        nsMgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");

        var item = doc.SelectSingleNode("//item") ?? doc.SelectSingleNode("//atom:entry", nsMgr);
        if (item == null) return null;

        // RSS format
        var title = item.SelectSingleNode("title")?.InnerText
                 ?? item.SelectSingleNode("atom:title", nsMgr)?.InnerText;

        var link = item.SelectSingleNode("link")?.InnerText
                ?? item.SelectSingleNode("atom:link/@href", nsMgr)?.Value
                ?? item.SelectSingleNode("atom:link", nsMgr)?.Attributes?["href"]?.Value;

        var description = item.SelectSingleNode("description")?.InnerText
                       ?? item.SelectSingleNode("atom:summary", nsMgr)?.InnerText
                       ?? item.SelectSingleNode("atom:content", nsMgr)?.InnerText
                       ?? "";

        if (string.IsNullOrWhiteSpace(title)) return null;

        // Clean the description
        description = CleanHtml(description);
        description = TruncateDescription(description, 280);

        return new NewsItem(
            Title: title.Trim(),
            Link: link?.Trim() ?? "",
            Description: description,
            Category: feed.Category,
            CategoryEmoji: feed.CategoryEmoji
        );
    }

    /// <summary>
    /// Strips HTML tags and decodes HTML entities from text.
    /// </summary>
    private static string CleanHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return "";

        // Remove HTML tags
        var text = Regex.Replace(html, "<[^>]+>", " ");

        // Decode common HTML entities
        text = text.Replace("&amp;", "&")
                   .Replace("&lt;", "<")
                   .Replace("&gt;", ">")
                   .Replace("&quot;", "\"")
                   .Replace("&#39;", "'")
                   .Replace("&nbsp;", " ")
                   .Replace("&#8217;", "'")
                   .Replace("&#8216;", "'")
                   .Replace("&#8220;", "\"")
                   .Replace("&#8221;", "\"");

        // Collapse multiple whitespace into single spaces
        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }

    /// <summary>
    /// Truncates description to maxLength characters, ending at a word boundary.
    /// </summary>
    private static string TruncateDescription(string text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLength) return text;

        var truncated = text[..maxLength];
        var lastSpace = truncated.LastIndexOf(' ');

        if (lastSpace > maxLength / 2)
            truncated = truncated[..lastSpace];

        return truncated.TrimEnd('.', ',', ' ') + "...";
    }
}
