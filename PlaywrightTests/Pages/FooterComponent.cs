using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class FooterComponent
    {
        private readonly IPage _page;

        public FooterComponent(IPage page)
        {
            _page = page;
        }

        // Locate all visible policy links in the footer
        private ILocator PoliciesList(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/']");


       public async Task GoToPoliciesPage(string langCode)
    {
            // Use the dynamic locator
            var policiesList = PoliciesList(langCode);

            await policiesList.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Construct the expected URI dynamically based on the language code
            var expectedURI = $"https://tokero.dev/{langCode.ToLower()}/policies/";
            var currentURI = _page.Url;

            if (!currentURI.Equals(expectedURI, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Unexpected URI: {currentURI}. Expected: {expectedURI}");
            }
            // Validate that the page title contains the expected text
            Console.WriteLine($"For Language ({langCode}) the URL is : {expectedURI}, expected: {currentURI}");

        }
        
    }
}
