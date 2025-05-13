using NUnit.Framework;
using Microsoft.Playwright;
using PlaywrightTests.Pages;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using Serilog;
using System.Text.RegularExpressions;
using PlaywrightTests.Helpers;
using static NUnit.Framework.Assert;
using NUnit.Allure.Core;   
namespace PlaywrightTests.Tests
{
    [TestFixture]
    [AllureNUnit]   
    [AllureSuite("Policy Tests")]
    public class PolicyTests
    {
        private IBrowser _browser = null!;
        private IBrowserContext _context = null!;
        private IPage _page = null!;
        private HomePage _homePage = null!;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            Logger.Configure();
            Log.Information("Test execution started.");
        }

        [SetUp]
        public async Task Setup()
        {
            try
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

                Log.Information("Test setup completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during test setup.");
                throw;
            }
        }

        [TearDown]
        public async Task Cleanup()
        {
            try
            {
                // Attach video to Allure report
                if (_page != null)
                {
                    await _page.CloseAsync();
                }

                if (_context != null)
                {
                    await _context.DisposeAsync();
                }

                if (_browser != null)
                {
                    await _browser.DisposeAsync();
                }

                Log.Information("Test cleanup completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during test cleanup.");
                throw;
            }
        }

        [OneTimeTearDown]
        public void ClassCleanup()
        {
            Log.Information("Test execution completed.");
            Logger.CloseAndFlush(); // Flush and close Serilog
        }

        [Test]
        [AllureTag("Regression", "Smoke")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureOwner("QA Team")]
        [AllureDescription("This test validates all policy pages for a given language.")]
        [TestCase("EN")]
        public async Task ValidateAllPoliciesPages(string langCode)
        {

            try
            {
                Log.Information($"Starting test: ValidateAllPoliciesPages for language: {langCode}");

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
            catch (Exception ex)
            {
                Log.Error(ex, "Error during test execution.");
                throw;
            }
        }

        [Test]
        [AllureTag("Regression", "MultiBrowser")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureOwner("QA Team")]
        [AllureDescription("This test validates policy pages across multiple browsers.")]
        [TestCase("chromium", "EN")]
        [TestCase("firefox", "EN")]
        [TestCase("webkit", "EN")]
        public async Task ValidatePoliciesPages_MultiBrowser(string browserType, string langCode)
        {

            try
            {
                Log.Information($"Starting test: ValidatePoliciesPages_MultiBrowser for browser: {browserType}, language: {langCode}");

                var playwright = await Playwright.CreateAsync();

                // Launch the specified browser
                _browser = browserType switch
                {
                    "chromium" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
                    "firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
                    "webkit" => await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
                    _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
                };

                // Create a new browser context and page
                _context = await _browser.NewContextAsync();
                _page = await _context.NewPageAsync();
                _homePage = new HomePage(_page);

                // Navigate to the home page
                await _homePage.GoToAsync();

                // Switch language only if it's not the default ("EN")
                if (!langCode.Equals("EN", StringComparison.OrdinalIgnoreCase))
                {
                    await _homePage.SwitchFromToLanguageAsync("EN", langCode);
                }

                // Navigate to the policies page
                var footer = new FooterComponent(_page);
                await footer.GoToPoliciesPage(langCode);

                // Validate all policy pages
                var policiesPage = new PoliciesPage(_page);
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
            catch (Exception ex)
            {
                Log.Error(ex, "Error during test execution.");
                throw;
            }
        }
    }
}
