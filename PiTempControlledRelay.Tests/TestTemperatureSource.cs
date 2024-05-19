using Microsoft.Extensions.Logging;

namespace PiTempControlledRelay.Tests;

internal class TestTemperatureSource : ITemperature
{
    public Dictionary<byte, double?> Temperatures { get; } = [];

    public Task<double?> GetTemperatureF(string ip, int port, byte sensorId, ILogger logger)
    {
        return Task.FromResult(Temperatures[sensorId]);
    }
}
