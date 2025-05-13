using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class WebinarsPage
    {
        private readonly IPage _page;

        public WebinarsPage(IPage page)
        {
            _page = page;
        }

        public ILocator PreviewImage => _page.GetByRole(AriaRole.Img, new() { Name = "How to Build a Profitable" });
        public ILocator RegisterNowLink => _page.GetByRole(AriaRole.Link, new() { Name = "Register now!" }).Nth(1);



    }
}
