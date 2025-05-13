using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class ExchangePage
    {
        private readonly IPage _page;

        public ExchangePage(IPage page)
        {
            _page = page;
        }

        public ILocator SearchTextbox => _page.GetByRole(AriaRole.Textbox, new() { Name = "Search..." });
        public ILocator NoResultsHeading => _page.GetByRole(AriaRole.Heading, new() { Name = "No results for 'SOL'" });


    }
}
