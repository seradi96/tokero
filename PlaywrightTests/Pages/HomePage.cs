using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class HomePage
    {
        private readonly IPage _page;
        private readonly string _url = "https://tokero.dev/en/";

        public HomePage(IPage page)
        {
            _page = page;
        }

        // Locators
        private ILocator LanguageSwitcher => _page.GetByRole(AriaRole.Button, new() { Name = "en flag EN", Exact = true });
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
                _ => "en flag EN" // default fallback
            };

            return _page.GetByRole(AriaRole.Button, new() { Name = expectedName, Exact = true });
        }

    }
}
