using BigMission.VictronSdk.Modbus;
using Microsoft.Extensions.Logging;

namespace PiTempControlledRelay;

internal class VictronTemp : ITemperature
{
    public async Task<double?> GetTemperatureF(string ip, int port, byte sensorId, ILogger logger)
    {
        return await TemperatureSource.GetTemperatureF(ip, port, sensorId, logger);
    }
}
