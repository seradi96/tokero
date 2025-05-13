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
    [AllureSuite("Performance Tests")]
    public class PerformanceTests
    {
        private IBrowser _browser = null!;
        private IPage _page = null!;
        private HomePage _homePage = null!;
        private IBrowserContext _context = null!;

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
        [AllureTag("Performance", "MultiBrowser")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureOwner("Performance Team")]
        [AllureDescription("This test validates policy pages across multiple browsers with performance metrics.")]
        [TestCase("chromium", "EN")]
        [TestCase("firefox", "EN")]
        [TestCase("webkit", "EN")]
        public async Task ValidatePoliciesPages_MultiBrowserWithPerformance(string browserType, string langCode)
        {

            try
            {
                Log.Information($"Starting test: ValidatePoliciesPages_MultiBrowserWithPerformance for browser: {browserType}, language: {langCode}");

                var playwright = await Playwright.CreateAsync();

                // Launch the specified browser
                _browser = browserType switch
                {
                    "chromium" => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }),
                    "firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }),
                    "webkit" => await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }),
                    _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
                };

                // Create a new browser context with HAR recording
                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    RecordHarPath = $"har_{browserType}_{langCode}.har"
                });

                // Start tracing
                await _context.Tracing.StartAsync(new TracingStartOptions
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });

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

                // Measure performance metrics
                var startTime = DateTime.Now;
                await policiesPage.ValidateAllPolicyPagesAsync(langCode, policyLocators);
                var endTime = DateTime.Now;

                Console.WriteLine($"Total validation time for browser: {browserType}, language: {langCode}: {(endTime - startTime).TotalMilliseconds} ms");

                // Stop tracing and save the trace
                await _context.Tracing.StopAsync(new TracingStopOptions
                {
                    Path = $"trace_{browserType}_{langCode}.zip"
                });

                // Stop HAR recording
                await _context.CloseAsync();

                Console.WriteLine($"Performance test completed for browser: {browserType}, language: {langCode}. Trace and HAR files saved.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during test execution.");
                throw;
            }
        }
    }
}
