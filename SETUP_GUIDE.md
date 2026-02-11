# GitHub Actions Setup Guide

## Step 1: Push Your Code to GitHub

First, commit and push your changes to GitHub:
```bash
git add .
git commit -m "Fix LinkedIn API payload and add configuration support"
git push origin main
```

## Step 2: Configure GitHub Secrets

1. Go to your GitHub repository: https://github.com/MohammadAlMokdad/LinkedIn-AutoPoster

2. Navigate to **Settings** → **Secrets and variables** → **Actions**

3. Click **New repository secret** and add the following secrets:

### Required Secrets:

**LINKEDIN_TOKEN**
- Your LinkedIn API access token
- Get it from: https://www.linkedin.com/developers/
- Must have `w_member_social` scope

**LINKEDIN_AUTHOR_URN**
- Your LinkedIn author URN in format: `urn:li:person:XXXXXX`
- Get it by calling: `GET https://api.linkedin.com/v2/me`

**RSS_FEED_URL**
- URL of the RSS feed you want to fetch news from
- Example: `https://time.com/newsfeed/feed/`

### Optional Secret:

**VISIBILITY**
- Post visibility: `CONNECTIONS` or `PUBLIC`
- Default: `CONNECTIONS` (if not set)

## Step 3: Trigger the Workflow

### Option A: Manual Trigger (Recommended for testing)

1. Go to the **Actions** tab in your GitHub repository
2. Click on "Weekly LinkedIn Poster" workflow
3. Click the **Run workflow** button
4. Select branch (usually `main`)
5. Click the green **Run workflow** button

### Option B: Wait for Scheduled Run

The workflow is scheduled to run every Monday at 10:00 UTC automatically.

## Step 4: Monitor the Workflow

1. Go to the **Actions** tab
2. Click on the active workflow run
3. You can see the logs in real-time
4. Look for "LinkedIn post successful!" in the output

## Troubleshooting

### Workflow Fails with "Missing configuration"
- Ensure all required secrets are set correctly
- Verify secret names match exactly (case-sensitive)
- Check spelling of secret names

### LinkedIn Post Fails
- Access token may have expired - generate a new one
- Verify Author URN format is correct
- Ensure access token has `w_member_social` scope
- Check for rate limiting or API errors

### Build Fails
- The workflow uses .NET 10.0
- Check the build logs for specific errors
- Ensure all dependencies are compatible

## Notes

- Your secrets are encrypted and only used during workflow execution
- The workflow will post the latest news from your RSS feed
- Duplicate posts are automatically rejected by LinkedIn
- You can manually trigger the workflow at any time
- The workflow will run automatically every Monday at 10:00 UTC

## Testing

After setting up the secrets, trigger a manual workflow run to test everything works correctly. Check the logs to verify:
1. Project builds successfully
2. Configuration is loaded
3. RSS feed is fetched
4. LinkedIn post is created successfully