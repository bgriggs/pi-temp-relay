using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PiTempControlledRelay.Tests;

internal class TestService : Service
{
    public TestService(IConfiguration configuration, ILoggerFactory loggerFactory, IRelayControl relayControl, ITemperature temperatureSource) 
        : base(configuration, loggerFactory, relayControl, temperatureSource)
    {
        
    }

    public async Task RunExecute(CancellationToken cancellationToken)
    {
        await ExecuteAsync(cancellationToken);
    }
}
