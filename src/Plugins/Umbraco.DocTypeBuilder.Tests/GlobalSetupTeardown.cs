using Umbraco.Cms.Tests.Integration.Testing;
using NUnit.Framework;

/// <summary>
/// Global setup and teardown for Umbraco integration tests.
/// This class is required by Umbraco's integration testing framework.
/// It should not have a namespace as per Umbraco documentation.
/// </summary>
[SetUpFixture]
public class CustomGlobalSetupTeardown
{
    private static GlobalSetupTeardown _setupTearDown;

    [OneTimeSetUp]
    public void SetUp()
    {
        _setupTearDown = new GlobalSetupTeardown();
        _setupTearDown.SetUp();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _setupTearDown.TearDown();
    }
}