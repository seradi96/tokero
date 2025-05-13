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
    [AllureSuite("Language Tests")]
    public class LanguageTests
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
        [AllureDescription("This test validates the ability to switch languages and verify mission text.")]
        public async Task CanSwitchLanguages()
        {
            var lifecycle = AllureLifecycle.Instance;

            try
            {
                Log.Information("Starting test: CanSwitchLanguages");

                await _homePage.GoToAsync();

                // Define the language sequence and expected mission texts
                var languageSequence = new[]
                {
                    new { From = "EN", To = "RO", ExpectedText = GlobalTexts.MissionTextRO },
                    new { From = "RO", To = "FR", ExpectedText = GlobalTexts.MissionTextFR },
                    new { From = "FR", To = "EN", ExpectedText = GlobalTexts.MissionTextEN }
                };

                // Validate initial mission text in English
                var initialText = await _page.Locator("p.home_tokeroMission__IhReX").TextContentAsync();
                Log.Information($"Mission Text (EN): {initialText}");
                NUnit.Framework.Assert.That(initialText, Is.EqualTo(GlobalTexts.MissionTextEN), "The mission text in English does not match the expected value.");

                // Loop through the language sequence and validate mission text
                foreach (var step in languageSequence)
                {
                    await _homePage.SwitchFromToLanguageAsync(step.From, step.To);
                    var missionText = await _page.Locator("p.home_tokeroMission__IhReX").TextContentAsync();
                    Log.Information($"Mission Text ({step.To}): {missionText}");
                    NUnit.Framework.Assert.That(missionText, Is.EqualTo(step.ExpectedText), $"The mission text in {step.To} does not match the expected value.");
                }

                lifecycle.UpdateTestCase(x => x.status = Status.passed);
            }
            catch (Exception ex)
            {
                AllureLifecycle.Instance.UpdateTestCase(x => x.status = Status.failed);
                Log.Error(ex, "Error during test execution.");
                throw;
            }
        }

        [Test]
        [AllureTag("Regression", "Smoke")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureOwner("QA Team")]
        [AllureDescription("This test validates navigation to the policies page in multiple languages.")]
        [TestCase("EN")]
        [TestCase("RO")]
        [TestCase("FR")]
        public async Task GoToPoliciesPage_MultiLanguage(string langCode)
        {

            try
            {
                Log.Information($"Starting test: GoToPoliciesPage_MultiLanguage for language: {langCode}");

                await _homePage.GoToAsync();

                // Switch language only if it's not the default ("EN")
                if (!langCode.Equals("EN", StringComparison.OrdinalIgnoreCase))
                {
                    await _homePage.SwitchFromToLanguageAsync("EN", langCode);
                }

                // Navigate to the policies page and validate the heading
                var footer = new FooterComponent(_page);
                await footer.GoToPoliciesPage(langCode);

                var headingText = await _page.Locator("h1, h2").First.TextContentAsync();
                Log.Information($"Heading Text ({langCode}): {headingText}");

                // Assert that the heading text is not empty and matches the expected value
                NUnit.Framework.Assert.That(headingText, Is.Not.Null.Or.Empty, $"No heading found for language: {langCode}");
                NUnit.Framework.Assert.That(headingText, Is.EqualTo(GlobalTexts.PoliciesPageHeadings[langCode]), $"The heading text for {langCode} does not match the expected value.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during test execution.");
                throw;
            }
        }
    }
}
