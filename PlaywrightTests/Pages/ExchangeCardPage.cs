using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class ExchangeCardPage
    {
        private readonly IPage _page;

        public ExchangeCardPage(IPage page)
        {
            _page = page;
        }

        public ILocator DailyLimitText => _page.Locator("#next-layout");
        public ILocator EnterAmountInput => _page.GetByPlaceholder("Enter amount");
        public ILocator Button1000E => _page.GetByRole(AriaRole.Button, new() { Name = "1000€" });
        public ILocator ReceiveAmountInput => _page.Locator("input[name=\"receiveAmount\"]");
        public ILocator USDCButton => _page.GetByRole(AriaRole.Button, new() { Name = "USDC USDC " });
        public ILocator BTCMenuItem => _page.GetByRole(AriaRole.Menuitem, new() { Name = "BTC BTC" });
        public ILocator MastercardLogo => _page.GetByRole(AriaRole.Img, new() { Name = "Mastercard logo" });



    }
}
