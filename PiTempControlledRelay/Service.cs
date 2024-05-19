using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PiTempControlledRelay;

public class Service : BackgroundService
{
    private readonly IConfiguration configuration;
    private readonly IRelayControl relayControl;
    private readonly ITemperature temperatureSource;

    private ILogger Logger { get; }

    public Service(IConfiguration configuration, ILoggerFactory loggerFactory, IRelayControl relayControl, ITemperature temperatureSource)
    {
        this.configuration = configuration;
        this.relayControl = relayControl;
        this.temperatureSource = temperatureSource;
        Logger = loggerFactory.CreateLogger(GetType().Name);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMs = configuration.GetValue<int>("TempCheckIntervalMs");
        Logger.LogInformation($"Starting service on interval: {intervalMs}ms");

        var ip = configuration.GetValue<string>("CerboIP") ?? "192.168.1.100";
        Logger.LogInformation($"Using IP: {ip}");
        var tempThresholdF = configuration.GetValue<double>("TempThresholdDegF");
        Logger.LogInformation($"Using temp threshold: {tempThresholdF}F");
        var tempThresholdOffDefF = configuration.GetValue<double>("TempThresholdOffDegF");
        Logger.LogInformation($"Using temp threshold off: {tempThresholdOffDefF}F");
        var sensorId = configuration.GetValue<byte>("SensorVRMInstance");
        Logger.LogInformation($"Using VRM sensor ID: {sensorId}");
        var enableAmbientCheck = configuration.GetValue<bool>("EnableAboveAmbient");
        Logger.LogInformation($"Enable above ambient check: {enableAmbientCheck}");
        var relayControlPin = configuration.GetValue<int>("RelayControlPin");
        relayControl.GpioPin = relayControlPin;
        Logger.LogInformation($"Using relay control GPIO pin: {relayControlPin}");

        while (!stoppingToken.IsCancellationRequested)
        {
            var temperature = await temperatureSource.GetTemperatureF(ip, 502, sensorId, Logger);
            var aboveAmbient = true;
            if (enableAmbientCheck)
            {
                var ambientSensorId = configuration.GetValue<byte>("AmbientTempSensorVRMInstance");
                var ambientTemperature = temperatureSource.GetTemperatureF(ip, 502, ambientSensorId, Logger).Result ?? 0;
                Logger.LogInformation($"Ambient temperature: {ambientTemperature}F");

                aboveAmbient = temperature > ambientTemperature;
                Logger.LogDebug($"Is temp above ambient: {aboveAmbient}");
            }

            // Turn on relay if temperature is above threshold and above ambient
            if (!relayControl.IsOn)
            {
                if (temperature >= tempThresholdF && aboveAmbient)
                {
                    relayControl.TurnOn();
                    Logger.LogInformation($"Relay turned on: {temperature} > {tempThresholdF} && {aboveAmbient}");
                }
            }
            else // Relay is on
            {
                // Turn off relay if temperature is below threshold
                if (temperature <= tempThresholdOffDefF)
                {
                    relayControl.TurnOff();
                    Logger.LogInformation($"Relay turned off: {temperature} <= {tempThresholdOffDefF}");
                }
                else if (!aboveAmbient)
                {
                    // It is hotter outside than inside, turn off relay
                    relayControl.TurnOff();
                    Logger.LogInformation($"Relay turned off: {aboveAmbient}");
                }
            }

            if (intervalMs < 0)
            {
                break; // For unit testing
            }
            await Task.Delay(intervalMs, stoppingToken);
        }
    }
}
