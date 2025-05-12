using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using PlaywrightTests.Pages;
using PlaywrightTests.Helpers;


namespace PlaywrightTests.Tests
{
    [TestClass]
    public class TokeroTests 
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

            // Initial title (EN)
            var initialText = await _page.Locator("p.home_tokeroMission__IhReX").TextContentAsync();
            Console.WriteLine($"Mission Text (EN): {initialText}");
            Assert.AreEqual(GlobalTexts.MissionTextEN, initialText, "The mission text in English does not match the expected value.");
            
            // Switch to RO
            await _homePage.SwitchFromToLanguageAsync("EN", "RO");
            var missionTextRo = await _page.Locator("p.home_tokeroMission__IhReX").TextContentAsync();
            Console.WriteLine($"Mission Text (RO): {missionTextRo}");
            Assert.AreEqual(GlobalTexts.MissionTextRO, initialText, "The mission text in English does not match the expected value.");

            // Switch to FR
            await _homePage.SwitchFromToLanguageAsync("RO", "FR");
            var missionTextFr = await _page.Locator("p.home_tokeroMission__IhReX").TextContentAsync();
            Console.WriteLine($"Mission Text (FR): {missionTextFr}");
            Assert.AreEqual(GlobalTexts.MissionTextFR, initialText, "The mission text in English does not match the expected value.");
            
            // Switch back to EN
            await _homePage.SwitchFromToLanguageAsync("FR", "EN");
            var newMissionTextEn = await _page.Locator("p.home_tokeroMission__IhReX").TextContentAsync();
            Console.WriteLine($"Mission Text (EN Restored): {newMissionTextEn}");
            Assert.AreEqual(GlobalTexts.MissionTextEN, initialText, "The mission text in English does not match the expected value.");
        }
    }
}
