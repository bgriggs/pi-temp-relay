using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace PiTempControlledRelay.Tests;

[TestClass]
public class ServiceTests
{
    private TestRelay? relayControl;
    private TestTemperatureSource? temperatureSource;
    private IConfiguration? configuration;
    private Mock<ILoggerFactory>? loggerFactoryMock;
    private Mock<ILogger>? loggerMock;
    private TestService? service;
    private Dictionary<string, string?> configValues = new()
    {
        { "TempCheckIntervalMs", "-1" }, // Make negative to break while loop
        { "CerboIP", "192.168.1.100" },
        { "SensorVRMInstance", "1" },
        { "AmbientTempSensorVRMInstance", "2" },
        { "RelayControlPin", "1" },
        { "EnableAboveAmbient", "true" }
    };

    [TestInitialize]
    public void Setup()
    {
        relayControl = new TestRelay();
        temperatureSource = new TestTemperatureSource();
        loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

        InitializeServiceWithConfiguration();
    }

    private void InitializeServiceWithConfiguration()
    {
        configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        service = new TestService(configuration, loggerFactoryMock!.Object, relayControl!, temperatureSource!);
    }

    [TestMethod]
    public async Task ShouldTurnOnRelay_WhenTemperatureAtThresholdAndAboveAmbient()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 80.0;
        temperatureSource!.Temperatures[2] = 75.0;

        // Act
        await service!.RunExecute(CancellationToken.None);

        Assert.AreEqual(true, relayControl!.IsOn);
    }

    [TestMethod]
    public async Task ShouldTurnOnRelay_WhenTemperatureAboveThresholdAndAboveAmbient()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 81.0;
        temperatureSource!.Temperatures[2] = 75.0;

        // Act
        await service!.RunExecute(CancellationToken.None);

        Assert.AreEqual(true, relayControl!.IsOn);
    }

    [TestMethod]
    public async Task ShouldTurnOnRelay_WhenTemperatureBelowThresholdAndAboveAmbient()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 79.9;
        temperatureSource!.Temperatures[2] = 75.0;

        // Act
        await service!.RunExecute(CancellationToken.None);

        // Assert
        Assert.AreEqual(false, relayControl!.IsOn);
    }

    [TestMethod]
    public async Task ShouldTurnOnRelay_WhenTemperatureAboveThresholdAndBelowAmbient()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 81.0;
        temperatureSource!.Temperatures[2] = 82.0;

        // Act
        await service!.RunExecute(CancellationToken.None);

        // Assert
        Assert.AreEqual(false, relayControl!.IsOn);
    }

    [TestMethod]
    public async Task ShouldTurnOnRelay_WhenTemperatureAboveThresholdAndAtAmbient()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 81.0;
        temperatureSource!.Temperatures[2] = 81.0;

        // Act
        await service!.RunExecute(CancellationToken.None);

        // Assert
        Assert.AreEqual(false, relayControl!.IsOn);
    }

    [TestMethod]
    public async Task ShouldTurnOffRelay_WhenTemperatureBelowThreshold()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        configValues["TempThresholdOffDegF"] = "75.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 74.0;
        temperatureSource!.Temperatures[2] = 70.0;

        // Act
        relayControl!.TurnOn();
        await service!.RunExecute(CancellationToken.None);

        // Assert
        Assert.AreEqual(false, relayControl!.IsOn);
    }

    [TestMethod]
    public async Task ShouldTurnOffRelay_WhenTemperatureAtThreshold()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        configValues["TempThresholdOffDegF"] = "75.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 75.0;
        temperatureSource!.Temperatures[2] = 70.0;

        // Act
        relayControl!.TurnOn();
        await service!.RunExecute(CancellationToken.None);

        // Assert
        Assert.AreEqual(false, relayControl!.IsOn);
    }

    [TestMethod]
    public async Task ShouldTurnOffRelay_WhenTemperatureAboveThresholdAndNotAboveAmbient()
    {
        // Arrange
        configValues["TempThresholdDegF"] = "80.0";
        InitializeServiceWithConfiguration();
        temperatureSource!.Temperatures[1] = 81.0;
        temperatureSource!.Temperatures[2] = 85.0;

        // Act
        relayControl!.TurnOn();
        await service!.RunExecute(CancellationToken.None);

        // Assert
        Assert.AreEqual(false, relayControl!.IsOn);
    }
}