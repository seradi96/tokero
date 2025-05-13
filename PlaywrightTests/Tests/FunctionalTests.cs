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
    [AllureSuite("Functional Tests")]
    public class FunctionalTests
    {
        private IBrowser _browser = null!;
        private IPage _page = null!;
        private IBrowserContext _context = null!;
        private HomePage _homePage = null!;
        private ExchangeCardPage _exchangeCardPage = null!;
        private WebinarsPage _webinarsPage = null!;
        private InfoCoinsPage _infoCoinsPage = null!;
        private ExchangePage _exchangePage = null!;

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
                    Headless = false
                });

                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    RecordVideoDir = "videos/",
                    RecordHarPath = "trace.har"
                });

                _page = await _context.NewPageAsync();
                _homePage = new HomePage(_page);
                _exchangeCardPage = new ExchangeCardPage(_page);
                _webinarsPage = new WebinarsPage(_page);
                _infoCoinsPage = new InfoCoinsPage(_page);
                _exchangePage = new ExchangePage(_page);

                await _homePage.GoToAsync();

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
        [AllureDescription("This test validates the 'Buy Instantly' functionality.")]
        public async Task ValidateFunctionalities()
        {
            try
            {
                Log.Information("Starting test: ValidateFunctionalities");

                // Buy instantly
                await _homePage.BuyInstantlyLink.ClickAsync();
                await _exchangeCardPage.DailyLimitText.ClickAsync();
                await Assertions.Expect(_exchangeCardPage.DailyLimitText).ToContainTextAsync("Please note that your daily credit card deposit limit is currently 10000 Euro.");
                Log.Information("Buy instantly functionality validated.");

                // Enter amount and validate receive amount
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await _exchangeCardPage.EnterAmountInput.ClickAsync();
                await _exchangeCardPage.EnterAmountInput.FillAsync("1500");
                await _exchangeCardPage.ReceiveAmountInput.ClickAsync();
                var actualReceiveAmount = await _exchangeCardPage.ReceiveAmountInput.InputValueAsync();
                NUnit.Framework.Assert.That(actualReceiveAmount, Is.Not.Null.Or.Empty, "The receive amount input is empty.");
                NUnit.Framework.Assert.That(Regex.IsMatch(actualReceiveAmount, @"^\d+(\.\d{1,2})?$"),Is.True, $"The receive amount '{actualReceiveAmount}' is not in the expected numeric format.");
                await Assertions.Expect(_exchangeCardPage.ReceiveAmountInput).ToHaveValueAsync(actualReceiveAmount);
                Log.Information("Receive amount validation completed.");

                // Change currency to BTC
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await _exchangeCardPage.USDCButton.ClickAsync();
                await _exchangeCardPage.BTCMenuItem.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Log.Information("Coin changed.");

                await _exchangeCardPage.Button1000E.ClickAsync();
                var actualBTC = await _exchangeCardPage.ReceiveAmountInput.InputValueAsync();
                NUnit.Framework.Assert.That(actualBTC, Is.Not.Null.Or.Empty, "The receive amount input is empty.");
                NUnit.Framework.Assert.That(Regex.IsMatch(actualBTC, @"^\d+(\.\d{1,2})?$"),Is.True, $"The receive amount '{actualBTC}' is not in the expected numeric format.");
                await Assertions.Expect(_exchangeCardPage.ReceiveAmountInput).ToHaveValueAsync(actualBTC);
                Log.Information("Receive amount validation completed.");

                // Validate Mastercard logo
                await Assertions.Expect(_exchangeCardPage.MastercardLogo).ToBeVisibleAsync();
                await _exchangeCardPage.MastercardLogo.ScrollIntoViewIfNeededAsync();

                // Simulate user experience by navigating back
                await _page.GoBackAsync();
                await _page.GoBackAsync();
                await _page.GoBackAsync();
                Log.Information("Returned to the home page.");

                // Watch recording
                await _homePage.WatchRecordingLink.ClickAsync();
                await Assertions.Expect(_webinarsPage.PreviewImage).ToBeVisibleAsync();
                await _webinarsPage.RegisterNowLink.ScrollIntoViewIfNeededAsync();
                await Assertions.Expect(_webinarsPage.RegisterNowLink).ToBeVisibleAsync();
                Log.Information("Webinar Page visited.");

                await _page.GoBackAsync();

                // Search functionality
                await _homePage.ViewMoreLink.ClickAsync();
                await _exchangePage.SearchTextbox.ClickAsync();
                await _exchangePage.SearchTextbox.FillAsync("SOL");
                await Assertions.Expect(_exchangePage.NoResultsHeading).ToBeVisibleAsync();
                Log.Information("Search for SOL completed.");

                await _page.GoBackAsync();

                // Solana link
                await _homePage.SolanaLink.ClickAsync();
                await Assertions.Expect(_infoCoinsPage.InfoMission).ToContainTextAsync("At TOKERO, our mission is to make crypto accessible to everyone in a simple, fast and safe way.");
                await Assertions.Expect(_infoCoinsPage.ExchangeRateSOL).ToBeVisibleAsync();
                Log.Information("Visited Solana page.");

                await _page.GoBackAsync();

                // Subscribe to newsletter
                await _homePage.EmailTextbox.ClickAsync();
                await _homePage.EmailTextbox.FillAsync("test@gmail.com");
                await _homePage.SubscribeButton.ClickAsync();
                await Assertions.Expect(_homePage.SubscriptionSuccessMessage).ToContainTextAsync("You have succesfully subscribed to the newsletter! 🎉");
                Log.Information("Subscription to newsletter completed.");
                Log.Information("Test: ValidateFunctionalities completed successfully.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during test execution.");
                throw;
            }
        }
    }
}
