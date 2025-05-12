using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using PlaywrightTests.Pages;
using PlaywrightTests.Helpers;


namespace PlaywrightTests.Tests
{
    [TestClass]
    public class LanguageTests 
    {
        private IBrowser _browser = null!;
        private IBrowserContext _context = null!;
        private IPage _page = null!;
        private HomePage _homePage = null!;

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

        [TestCleanup]
        public async Task Cleanup()
        {
            await _context.CloseAsync();
            await _browser.CloseAsync();
        }

        [TestMethod]
        public async Task CanSwitchLanguages()
        {
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
            Console.WriteLine($"Mission Text (EN): {initialText}");
            Assert.AreEqual(GlobalTexts.MissionTextEN, initialText, "The mission text in English does not match the expected value.");

            // Loop through the language sequence and validate mission text
            foreach (var step in languageSequence)
            {
                await _homePage.SwitchFromToLanguageAsync(step.From, step.To);
                var missionText = await _page.Locator("p.home_tokeroMission__IhReX").TextContentAsync();
                Console.WriteLine($"Mission Text ({step.To}): {missionText}");
                Assert.AreEqual(step.ExpectedText, missionText, $"The mission text in {step.To} does not match the expected value.");
            }
        }

        [DataTestMethod]
        [DataRow("EN")]
        [DataRow("RO")]
        [DataRow("FR")]
        public async Task GoToPoliciesPage_MultiLanguage(string langCode)
        {
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
            Console.WriteLine($"Heading Text ({langCode}): {headingText}");
            // Assert that the heading text is not empty and matches the expected value
            Assert.IsFalse(string.IsNullOrWhiteSpace(headingText), $"No heading found for language: {langCode}");
            Assert.AreEqual(GlobalTexts.PoliciesPageHeadings[langCode], headingText, $"The heading text for {langCode} does not match the expected value.");
        }




    }
}
