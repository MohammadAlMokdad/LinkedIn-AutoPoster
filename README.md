# LinkedIn AutoPoster

A C# application that automatically posts news updates from an RSS feed to LinkedIn using the LinkedIn API.

## Features

- 📰 Fetches latest news from any RSS feed
- 🚀 Posts updates to LinkedIn automatically
- ⚙️ Supports both local configuration and environment variables
- 🔄 GitHub Actions integration for scheduled posts

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or higher
- LinkedIn Developer Account with API access
- LinkedIn Access Token
- LinkedIn Author URN (Person ID)

## Setup

### 1. Get LinkedIn API Credentials

1. Go to [LinkedIn Developer Portal](https://www.linkedin.com/developers/)
2. Create a new application or use an existing one
3. Generate an access token with the following scopes:
   - `w_member_social` - to create posts on behalf of the member
4. Note down your access token

### 2. Find Your LinkedIn Author URN

Your Author URN format is: `urn:li:person:YOUR_PERSON_ID`

To find your person ID:
- Use the LinkedIn API: `GET https://api.linkedin.com/v2/me`
- The response will contain your ID in the `id` field
- Construct your URN as `urn:li:person:{your_id}`

### 3. Configure the Application

#### Option A: Local Configuration (for development)

1. Create a file named `appsettings.json` in the project root
2. Copy the template below and fill in your credentials:

```json
{
  "LinkedIn": {
    "AccessToken": "YOUR_LINKEDIN_ACCESS_TOKEN",
    "AuthorUrn": "urn:li:person:YOUR_PERSON_ID",
    "Visibility": "CONNECTIONS"
  },
  "RssFeedUrl": "https://example.com/rss-feed"
}
```

**Visibility Options:**
- `CONNECTIONS` - Only visible to your connections (default)
- `PUBLIC` - Visible to everyone on LinkedIn

**Note:** The `appsettings.json` file is in `.gitignore` to protect your credentials.

#### Option B: Environment Variables (for production/GitHub Actions)

Set the following environment variables:
- `LINKEDIN_TOKEN` - Your LinkedIn API access token
- `LINKEDIN_AUTHOR_URN` - Your LinkedIn author URN
- `VISIBILITY` - Connections or PUBLIC (optional, default: CONNECTIONS)
- `RSS_FEED_URL` - URL of the RSS feed to fetch news from

### 4. Run the Application

Build and run the project:

```bash
dotnet build
dotnet run
```

The application will:
1. Fetch the latest news from your RSS feed
2. Create a LinkedIn post with the news content
3. Post to your LinkedIn profile

## Usage Examples

### Using a Tech News RSS Feed

```json
{
  "LinkedIn": {
    "AccessToken": "your_token_here",
    "AuthorUrn": "urn:li:person:123456",
    "Visibility": "PUBLIC"
  },
  "RssFeedUrl": "https://techcrunch.com/feed/"
}
```

### Using a Custom Blog RSS Feed

```json
{
  "LinkedIn": {
    "AccessToken": "your_token_here",
    "AuthorUrn": "urn:li:person:123456",
    "Visibility": "CONNECTIONS"
  },
  "RssFeedUrl": "https://yourblog.com/feed.xml"
}
```

## GitHub Actions

The project includes a GitHub Actions workflow for automated weekly posting.

### Quick Setup

See [SETUP_GUIDE.md](SETUP_GUIDE.md) for detailed instructions on configuring GitHub Actions.

### Setup GitHub Secrets

1. Go to your GitHub repository settings
2. Navigate to **Secrets and variables** > **Actions**
3. Add the following secrets:
   - `LINKEDIN_TOKEN` - Your LinkedIn API access token
   - `LINKEDIN_AUTHOR_URN` - Your LinkedIn author URN (e.g., `urn:li:person:XXXX`)
   - `RSS_FEED_URL` - URL of the RSS feed to fetch news from
   - `VISIBILITY` (optional) - `CONNECTIONS` or `PUBLIC`, default: `CONNECTIONS`

### Schedule

The workflow is scheduled to run every Monday at 10:00 UTC by default. You can modify the schedule in `.github/workflows/linkedin.yml`.

You can also manually trigger the workflow from the GitHub Actions tab.

## RSS Feed Format

The application expects standard RSS 2.0 format with the following structure:
- `<channel>` containing `<item>` elements
- Each `<item>` should have:
  - `<title>` - The news headline
  - `<link>` - URL to the full article

## Troubleshooting

### Missing Configuration Error

If you see "❌ Missing configuration", ensure:
- `appsettings.json` exists with all required fields, OR
- Environment variables are set properly

### LinkedIn Post Failed

Common issues:
- **Invalid Access Token**: Your token may have expired. Generate a new one.
- **Incorrect Author URN**: Verify your person ID using the LinkedIn API.
- **Insufficient Permissions**: Ensure your access token has the `w_member_social` scope.

### Build Errors

- Ensure you have .NET 10.0 SDK installed
- Run `dotnet restore` to restore dependencies

## Development

### Project Structure

```
LinkedInAutoPoster/
├── Program.cs              # Main entry point
├── LinkedInPoster.cs       # LinkedIn API integration
├── NewsFetcher.cs          # RSS feed fetching logic
├── LinkedInAutoPoster.csproj # Project configuration
├── appsettings.json        # Configuration template (gitignored)
├── .github/workflows/      # GitHub Actions workflow
└── README.md               # This file
```

## License

This project is provided as-is for educational and personal use.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.