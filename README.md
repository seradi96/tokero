# Tokero Playwright Tests

This repository contains automated tests for the Tokero platform, built using [Microsoft Playwright](https://playwright.dev/) and integrated with [NUnit](https://nunit.org/) for test execution and [Allure](https://docs.qameta.io/allure/) for reporting. The tests cover functional, performance, language, and policy validations across multiple browsers and languages.

---

## **Project Structure**

```
TokeroPlaywrightTests/
├── allureConfig.json          # Configuration for Allure reporting
├── PlaywrightTests.csproj     # Project file for .NET
├── allure-results/            # Directory for Allure test results
├── allure-report/             # Generated Allure reports
├── Helpers/                   # Helper classes (e.g., Logger, Reporter)
├── Pages/                     # Page Object Models for Tokero platform
├── Tests/                     # Test files (Functional, Performance, Language, Policy)
├── bin/                       # Compiled binaries and logs
├── obj/                       # Build artifacts
```

---

## **Implemented Features**

### **1. Functional Tests**
- **File**: `FunctionalTests.cs`
- **Purpose**: Validates core functionalities like "Buy Instantly," currency conversion, and newsletter subscription.
- **Key Features**:
  - Uses Playwright for browser automation.
  - Validates UI elements and input fields.
  - Integrated with Allure for detailed reporting, including video attachments.

### **2. Performance Tests**
- **File**: `PerformanceTests.cs`
- **Purpose**: Measures performance metrics for policy pages across multiple browsers.
- **Key Features**:
  - Records HAR files and traces for performance analysis.
  - Supports Chromium, Firefox, and WebKit browsers.
  - Integrated with Allure for performance reporting.

### **3. Language Tests**
- **File**: `LanguageTests.cs`
- **Purpose**: Validates language switching and mission text consistency.
- **Key Features**:
  - Tests multiple languages (EN, RO, FR).
  - Ensures mission text matches expected translations.
  - Integrated with Allure for multilingual test reporting.

### **4. Policy Tests**
- **File**: `PolicyTests.cs`
- **Purpose**: Validates all policy pages for a given language.
- **Key Features**:
  - Tests navigation and content validation for policy pages.
  - Supports multiple languages and browsers.
  - Integrated with Allure for detailed reporting.

---

## **Technologies Used**

### **1. Microsoft Playwright**
- Used for browser automation and UI testing.
- Supports multiple browsers (Chromium, Firefox, WebKit).
- Records HAR files and traces for performance analysis.

### **2. NUnit**
- Test framework for writing and executing tests.
- Provides attributes like `[Test]`, `[SetUp]`, `[TearDown]`, etc.
- Supports parameterized tests with `[TestCase]`.

### **3. Allure**
- Generates detailed test reports with screenshots, videos, and logs.
- Configured using `allureConfig.json`.
- Results are stored in the `allure-results` directory.

### **4. Serilog**
- Used for logging test execution details.
- Logs are stored in `bin/Debug/net8.0/logs`.

---

## **Setup Instructions**

### **1. Prerequisites**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (for Playwright dependencies)
- [Allure CLI](https://docs.qameta.io/allure/#_get_started)

### **2. Install Dependencies**
Run the following commands to install the required dependencies:
```bash
dotnet restore
playwright install
```

### **3. Configure Allure**
Ensure the `allureConfig.json` file is present in the root directory:
```json
{
    "allure": {
        "directory": "allure-results",
        "links": {
            "issue": "http://example.com/issue/{}/",
            "tms": "http://example.com/tms/{}/"
        }
    }
}
```

### **4. Run Tests**
Run all tests using:
```bash
dotnet test --logger:"console;verbosity=detailed"
```

Run specific tests (e.g., `ValidateFunctionalities`) using:
```bash
dotnet test --filter "FullyQualifiedName~PlaywrightTests.Tests.FunctionalTests.ValidateFunctionalities"
```

### **5. Generate Allure Report**
After running the tests, copy from the bin to the root the Allure results.

After running the tests, generate the Allure report:
```bash
allure generate allure-results --clean -o allure-report
```

Open the report in your browser:
```bash
allure open allure-report
```

---

## **Key Features in Tests**

### **1. Video Recording**
- Videos are recorded for the test.
- Configured using `RecordVideoDir` in Playwright's `NewContextAsync`.

### **2. HAR and Trace Recording**
- HAR files and traces are recorded for performance tests.
- Configured using `RecordHarPath` and `Tracing.StartAsync`.

### **3. Multilingual Support**
- Tests validate functionality in multiple languages (EN, RO, FR).
- Ensures UI consistency across translations.

### **4. Multi-Browser Testing**
- Tests are executed on Chromium, Firefox, and WebKit.
- Ensures compatibility across different browsers.

---

## **Logging**
- Logs are generated using Serilog and stored in `bin/Debug/net8.0/logs`.
- Example log file: test-log20250513.json.

---


## **Contact**
For any questions or issues, please contact the QA  at `andre.serban96@gmail.com`.


---

## **Tradeoffs Made During Development**

Throughout the development of this project, several tradeoffs were made to balance functionality, maintainability, and performance. Below is a summary of the key tradeoffs:

### **1. Use of Playwright for Browser Automation**
- **Decision**: Chose Playwright for browser automation.
- **Tradeoff**:
  - **Pros**: Playwright provides faster execution, built-in support for multiple browsers, and modern APIs for handling asynchronous operations.
  - **Cons**: Playwright has a smaller community compared to big ones like Selenium, which may limit the availability of third-party integrations and plugins.

---

### **2. NUnit as the Test Framework**
- **Decision**: Used NUnit for test execution instead of MSTest or xUnit.
- **Tradeoff**:
  - **Pros**: NUnit offers better support for parameterized tests (`[TestCase]`) and integrates seamlessly with Allure for reporting.
  - **Cons**: Migrating from MSTest required refactoring test attributes and assertions, which added initial development overhead.

---

### **3. Allure for Reporting**
- **Decision**: Integrated Allure for detailed test reporting.
- **Tradeoff**:
  - **Pros**: Allure provides rich, interactive reports with support for screenshots, videos, and logs.
  - **Cons**: Requires additional setup (e.g., `allureConfig.json`, CLI installation) and may not be as lightweight as simpler reporting tools.

---

### **4. Video and HAR Recording**
- **Decision**: Enabled video recording and HAR (HTTP Archive) generation for debugging and performance analysis.
- **Tradeoff**:
  - **Pros**: Provides valuable insights into test execution and network performance, making debugging easier.
  - **Cons**: Increases disk usage and test execution time, especially for long-running tests.

---

### **5. Multi-Browser Testing**
- **Decision**: Supported Chromium, Firefox, and WebKit for cross-browser compatibility.
- **Tradeoff**:
  - **Pros**: Ensures the application works consistently across major browsers.
  - **Cons**: Running tests on multiple browsers increases execution time and resource usage.

---

### **6. Multilingual Testing**
- **Decision**: Validated functionality in multiple languages (EN, RO, FR).
- **Tradeoff**:
  - **Pros**: Ensures the application is accessible and functional for users in different regions.
  - **Cons**: Requires additional test logic and maintenance for language-specific validations.

---

### **7. Headless vs. Headed Browsers**
- **Decision**: Used headed browsers during local development and headless browsers for performance also headless browsers will be used in CI environments.
- **Tradeoff**:
  - **Pros**: Headed browsers make debugging easier during development, while headless browsers improve performance in CI pipelines.
  - **Cons**: Differences in behavior between headed and headless modes may occasionally cause inconsistencies.

---

### **8. Logging with Serilog**
- **Decision**: Used Serilog for structured logging.
- **Tradeoff**:
  - **Pros**: Provides detailed logs for debugging and integrates well with .NET.
  - **Cons**: Adds slight overhead to test execution and requires additional configuration.

---

### **9. Test Coverage vs. Execution Time**
- **Decision**: Focused on comprehensive test coverage, including functional, performance, and multilingual tests.
- **Tradeoff**:
  - **Pros**: Ensures high-quality testing across various scenarios.
  - **Cons**: Increased test execution time, especially for multi-browser and multilingual tests.

---

### **10. Use of Page Object Model (POM)**
- **Decision**: Implemented the Page Object Model (POM) for test maintainability.
- **Tradeoff**:
  - **Pros**: Improves code reusability and readability by encapsulating page-specific logic.
  - **Cons**: Initial setup required additional development time to create and maintain page classes.

---

### **11. CI/CD Integration**
- **Decision**: Focused on local test execution with plans to integrate into CI/CD pipelines later.
- **Tradeoff**:
  - **Pros**: Allowed faster development and debugging during the initial phase.
  - **Cons**: Delayed the benefits of automated test execution in CI/CD environments.

---

### **12. Limited Third-Party Integrations**
- **Decision**: Minimized reliance on third-party libraries to keep the project lightweight.
- **Tradeoff**:
  - **Pros**: Reduces dependency management and potential compatibility issues.
  - **Cons**: Some advanced features (e.g., visual regression testing) were not implemented.

---
