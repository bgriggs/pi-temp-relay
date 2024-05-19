using Microsoft.Extensions.Logging;

namespace PiTempControlledRelay;

public interface ITemperature
{
    public Task<double?> GetTemperatureF(string ip, int port, byte sensorId, ILogger logger);
}
