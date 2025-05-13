using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class InfoCoinsPage
    {
        private readonly IPage _page;

        public InfoCoinsPage(IPage page)
        {
            _page = page;
        }

        public ILocator InfoMission => _page.Locator("blazor-coin-info_v5");
        public ILocator ExchangeRateSOL => _page.GetByText("Current exchange rates for SOL");


    }
}
