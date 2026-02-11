using System;
using System.Net.Http;
using System.Xml;

public class NewsFetcher
{
    private readonly string feedUrl;

    public NewsFetcher(string feedUrl)
    {
        this.feedUrl = feedUrl;
    }

    public string GetLatestNews()
    {
        try
        {
            using var client = new HttpClient();
            var xml = client.GetStringAsync(feedUrl).Result;

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var item = doc.SelectSingleNode("//item");
            if (item == null) return "No news available.";

            var titleNode = item.SelectSingleNode("title");
            var linkNode = item.SelectSingleNode("link");

            string title = titleNode?.InnerText ?? "No title";
            string link = linkNode?.InnerText ?? "";

            return $"📢 Weekly Tech Update: {title} {link}";
        }
        catch (Exception ex)
        {
            return $"Error fetching news: {ex.Message}";
        }
    }
}
