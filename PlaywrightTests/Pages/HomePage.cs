using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class HomePage
    {
        private readonly IPage _page;
        private readonly string _url = "https://tokero.dev/en/";
         // Locators
        public ILocator BuyInstantlyLink => _page.GetByRole(AriaRole.Link, new() { Name = "You can buy instantly via" });
        public ILocator WatchRecordingLink => _page.GetByRole(AriaRole.Link, new() { Name = "Watch the Recording" });
        public ILocator ViewMoreLink => _page.GetByRole(AriaRole.Link, new() { Name = "View more" }).First;
        public ILocator SolanaLink => _page.GetByRole(AriaRole.Link, new() { Name = "Solana SOL" });
        public ILocator EmailTextbox => _page.GetByRole(AriaRole.Textbox, new() { Name = "Please type your email" });
        public ILocator SubscribeButton => _page.GetByRole(AriaRole.Button, new() { Name = "Subscribe " });
        public ILocator SubscriptionSuccessMessage => _page.GetByRole(AriaRole.Contentinfo);


        public HomePage(IPage page)
        {
            _page = page;
        }

        // Locators
        private ILocator CookieAcceptButton => _page.GetByRole(AriaRole.Button, new() { Name = "Accept all cookies" });
        private ILocator CookieCloseButton => _page.GetByRole(AriaRole.Button, new() { Name = "✖" });


        // Actions
        public async Task GoToAsync()
        {
            await _page.GotoAsync(_url);

            if (await CookieAcceptButton.IsVisibleAsync())
                await CookieAcceptButton.ClickAsync();

            if (await CookieCloseButton.IsVisibleAsync())
                await CookieCloseButton.ClickAsync();

            // Wait for the page to settle (Blazor needs time to load dynamic content)
            //await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        
        public async Task<string> GetPageTitleAsync()
        {
            return await _page.TitleAsync();
        }

        public async Task SwitchFromToLanguageAsync(string currentLanguage, string languageCode)
        {

            // Open the dropdown
                var languageSwitcher = GetLanguageSwitcher(currentLanguage); // currentLanguage is what you're on now

            await languageSwitcher.ClickAsync();

            ILocator? languageOption = languageCode switch
            {
                "RO" => _page.GetByRole(AriaRole.Button, new() { Name = "ro flag RO" }),
                "FR" => _page.GetByRole(AriaRole.Button, new() { Name = "fr flag FR" }),
                "IT" => _page.GetByRole(AriaRole.Button, new() { Name = "it flag IT" }),
                "PL" => _page.GetByRole(AriaRole.Button, new() { Name = "pl flag PL" }),
                "PT" => _page.GetByRole(AriaRole.Button, new() { Name = "pt flag PT" }),
                "TR" => _page.GetByRole(AriaRole.Button, new() { Name = "tr flag TR" }),
                "EN" => _page.GetByRole(AriaRole.Button, new() { Name = "en flag EN" }),
                "DE" => _page.GetByRole(AriaRole.Button, new() { Name = "de flag DE" }),
                _ => throw new ArgumentException($"Unsupported language code: {languageCode}")
            };
            

            await languageOption.ClickAsync();
        }

        
        private ILocator GetLanguageSwitcher(string langCode)
        {
            string expectedName = langCode.ToLower() switch
            {
                "en" => "en flag EN",
                "ro" => "ro flag RO",
                "fr" => "fr flag FR",
                "it" => "it flag IT",
                "pl" => "pl flag PL",
                "pt" => "pt flag PT",
                "tr" => "tr flag TR",
                "de" => "de flag DE",
                _ => "en flag EN" // default fallback
            };

            return _page.GetByRole(AriaRole.Button, new() { Name = expectedName, Exact = true });
        }

    }
}
