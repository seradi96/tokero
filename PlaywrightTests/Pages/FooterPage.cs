using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class FooterPage
    {
        private readonly IPage _page;

        public FooterPage(IPage page)
        {
            _page = page;
        }

        // Locate all visible policy links in the footer
        private ILocator PolicyLinks => _page.Locator("footer a[href*='/policy'], footer a[href*='/terms'], footer a[href*='/privacy']");

        public async Task<List<string>> GetAllPolicyLinkHrefsAsync()
        {
            var hrefs = new List<string>();
            int count = await PolicyLinks.CountAsync();

            for (int i = 0; i < count; i++)
            {
                var link = PolicyLinks.Nth(i);
                var href = await link.GetAttributeAsync("href");
                if (!string.IsNullOrEmpty(href))
                {
                    hrefs.Add(href!);
                }
            }

            return hrefs;
        }

        public async Task<bool> IsPolicyPageValidAsync(string url)
        {
            var response = await _page.GotoAsync(url);
            if (response == null || !response.Ok)
                return false;

            // Wait for main content — adjust this based on actual content of those pages
            var heading = _page.Locator("h1, h2, article");
            return await heading.IsVisibleAsync();
        }
    }
}
