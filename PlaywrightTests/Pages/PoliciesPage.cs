using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class PoliciesPage
    {
        private readonly IPage _page;

        public PoliciesPage(IPage page)
        {
            _page = page;
        }

      // Locator for all policy links on the policies page
        public ILocator TermsPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/terms-of-service/']").First;
        public ILocator PrivacyPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/privacy/']").First;
        public ILocator CookiesPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/cookies/']").First;
        public ILocator FeesPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/fees/']");
        public ILocator KYCPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/kyc/']").First;
        public ILocator AMLPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/aml-countries/']");
        public ILocator ReferralsPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/referral-program/']");
        public ILocator AnswerPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/answering-times/']");
        public ILocator MinimumsPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/minimums-and-options/']");
        public ILocator GDPRPolicy(string langCode) => _page.Locator($"a[href='/{langCode.ToLower()}/policies/gdpr/']").First;


        // Method to validate all policy pages
        public async Task ValidateAllPolicyPagesAsync(string langCode, List<Func<string, ILocator>> policyLocators)
        {
            foreach (var getPolicyLocator in policyLocators)
            {
                // Get the locator for the current policy
                var policyLocator = getPolicyLocator(langCode);

                // Click the policy link
                await policyLocator.ClickAsync();

                // Wait for the page to load (network and DOM)
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await _page.WaitForSelectorAsync("h1, h2");

                // Validate the heading on the policy page
                var heading = await _page.Locator("h1, h2").First.TextContentAsync();
                Assert.IsFalse(string.IsNullOrWhiteSpace(heading), $"No heading found or heading is empty for policy: {policyLocator}");

                Console.WriteLine($"Validated policy page with heading: {heading}");

                // Navigate back to the policies page
                await _page.GoBackAsync(new PageGoBackOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle // Wait until the network is idle
                });

                await _page.WaitForSelectorAsync("div.text-center");

            }
        }
    
    }
}