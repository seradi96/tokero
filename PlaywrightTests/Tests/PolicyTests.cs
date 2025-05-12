using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using PlaywrightTests.Pages;
using PlaywrightTests.Helpers;


namespace PlaywrightTests.Tests
{
[TestClass]
public class PolicyTests 
{
    private IBrowser _browser= null!;
    private IPage _page= null!;
    private HomePage _homePage = null!;
    private IBrowserContext _context = null!;



    [TestInitialize]
    public async Task Setup()
    {
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
                Headless = false // Set to true in CI
        });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
                RecordVideoDir = "videos/",
                RecordHarPath = "trace.har"
        });

        _page = await _context.NewPageAsync();
        _homePage = new HomePage(_page);
    }

        [DataTestMethod]
        [DataRow("EN")]
        [DataRow("RO")]
        [DataRow("FR")]
        public async Task ValidateAllPoliciesPages(string langCode)
        {
            var policiesPage = new PoliciesPage(_page);
            await _homePage.GoToAsync();

            // Switch language only if it's not the default ("EN")
            if (!langCode.Equals("EN", StringComparison.OrdinalIgnoreCase))
            {
                await _homePage.SwitchFromToLanguageAsync("EN", langCode);
            }

            // Navigate to the policies page and validate the heading
            var footer = new FooterComponent(_page);
            await footer.GoToPoliciesPage(langCode);
            // List of policy locators
            var policyLocators = new List<Func<string, ILocator>>
            {
                policiesPage.TermsPolicy,
                policiesPage.PrivacyPolicy,
                policiesPage.CookiesPolicy,
                policiesPage.FeesPolicy,
                policiesPage.KYCPolicy,
                policiesPage.AMLPolicy,
                policiesPage.ReferralsPolicy,
                policiesPage.AnswerPolicy,
                policiesPage.MinimumsPolicy,
                policiesPage.GDPRPolicy
            };
                await policiesPage.ValidateAllPolicyPagesAsync(langCode, policyLocators);

        }
    
      [TestCleanup]
        public async Task Cleanup()
        {
            await _context.CloseAsync();
            await _browser.CloseAsync();
        }
}
}
