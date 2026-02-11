# LinkedIn AutoPoster

A C# application that automatically posts curated tech news, AI updates, and cybersecurity insights to LinkedIn every day — with beautifully formatted posts that drive engagement.

## ✨ Features

- 🤖 **Multi-source news aggregation** — Fetches from 7 curated RSS feeds covering AI, cybersecurity, dev tools, and tech news
- 🎨 **Beautiful post formatting** — Unicode bold headers, emojis, separators, and strategic hashtags
- 📅 **Daily automated posting** — Posts every day at 08:00 Lebanon time (06:00 UTC) via GitHub Actions
- 🔄 **Smart feed rotation** — Different category each day for content variety
- 🛡️ **API resilience** — New LinkedIn Posts API with automatic fallback to legacy UGC API
- ⚙️ **Flexible configuration** — Environment variables or JSON config

## 📡 Curated News Sources

| Category | Source |
|---|---|
| 🛡️ Cybersecurity & Hacking | The Hacker News |
| 🔒 Cybersecurity News | BleepingComputer |
| 🔐 Security Research | Krebs on Security |
| � Developer Tools | GitHub Blog |
| 🔬 AI Research | MIT News – AI |
| 🚀 Tech News | Ars Technica |
| 🤖 AI & Machine Learning | Google AI Blog |

## 📋 Post Format Preview

```
🔥 𝗗𝗮𝗶𝗹𝘆 𝗧𝗲𝗰𝗵 𝗜𝗻𝘀𝗶𝗴𝗵𝘁 — 🔐 Security Research

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📰 Article Title Here

Clean description of the article in 2-3 sentences...

🔗 Read the full article: https://example.com/article

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

💡 Stay ahead of the curve — Follow for daily tech insights!
♻️  Found this useful? Repost to help your network!

#TechNews #Innovation #DailyInsight
#CyberSecurity #InfoSec #Hacking #Privacy #ThreatIntelligence
```

## 🚀 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or higher
- LinkedIn Developer Account with API access
- LinkedIn Access Token with `w_member_social` scope
- LinkedIn Author URN (Person ID)

## ⚙️ Setup

### 1. Get LinkedIn API Credentials

1. Go to [LinkedIn Developer Portal](https://www.linkedin.com/developers/)
2. Create a new application or use an existing one
3. Generate an access token with the `w_member_social` scope
4. Note down your access token

### 2. Find Your LinkedIn Author URN

Your Author URN format is: `urn:li:person:YOUR_PERSON_ID`

To find your person ID:
- Use the LinkedIn API: `GET https://api.linkedin.com/v2/me`
- The response will contain your ID in the `id` field
- Construct your URN as `urn:li:person:{your_id}`

### 3. Configure the Application

#### Option A: Environment Variables (recommended for production)

```bash
export LINKEDIN_TOKEN="your_access_token"
export LINKEDIN_AUTHOR_URN="urn:li:person:your_id"
export VISIBILITY="CONNECTIONS"  # or "PUBLIC"
```

#### Option B: Local Configuration (for development)

Edit `appsettings.json`:

```json
{
  "LinkedIn": {
    "AccessToken": "YOUR_LINKEDIN_ACCESS_TOKEN",
    "AuthorUrn": "urn:li:person:YOUR_PERSON_ID",
    "Visibility": "CONNECTIONS"
  }
}
```

**Visibility Options:**
- `CONNECTIONS` — Only visible to your connections (default)
- `PUBLIC` — Visible to everyone on LinkedIn

### 4. Run the Application

```bash
dotnet build
dotnet run
```

## 🔄 GitHub Actions — Automated Daily Posts

### Setup GitHub Secrets

1. Go to your GitHub repository settings
2. Navigate to **Secrets and variables** > **Actions**
3. Add the following secrets:
   - `LINKEDIN_TOKEN` — Your LinkedIn API access token
   - `LINKEDIN_AUTHOR_URN` — Your LinkedIn author URN
   - `VISIBILITY` (optional) — `CONNECTIONS` or `PUBLIC` (default: `CONNECTIONS`)

### Schedule

The workflow runs **every day at 06:00 UTC (08:00 Lebanon time)**. You can also manually trigger it from the GitHub Actions tab.

## 🔧 Project Structure

```
LinkedInAutoPoster/
├── Program.cs              # Main entry point and orchestration
├── LinkedInPoster.cs       # LinkedIn API integration + post formatting
├── NewsFetcher.cs          # Multi-source RSS fetching + article extraction
├── LinkedInAutoPoster.csproj # Project configuration
├── appsettings.json        # Local config template (gitignored)
├── .github/workflows/      # GitHub Actions workflow
└── README.md               # This file
```

## 🐛 Troubleshooting

### LinkedIn Post Failed
- **Invalid Access Token** — Your token may have expired. Generate a new one.
- **Incorrect Author URN** — Verify your person ID using the LinkedIn API.
- **Insufficient Permissions** — Ensure your token has the `w_member_social` scope.
- **API Version Issue** — The app tries the new Posts API first, then falls back to legacy UGC API automatically.

### All Feeds Failed
- Some RSS feeds may be temporarily down. The app tries all 7 feeds in random order and uses the first one that succeeds.

### Build Errors
- Ensure you have .NET 10.0 SDK installed
- Run `dotnet restore` to restore dependencies

## 📄 License

This project is provided as-is for educational and personal use.

## 🤝 Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.