namespace PiTempControlledRelay.Tests;

internal class TestRelay : IRelayControl
{
    public int GpioPin { get; set; }

    public bool IsOn { get; private set; }

    public void InitializePin(int gpioPin)
    {
        GpioPin = gpioPin;
    }

    public void TurnOff()
    {
        IsOn = false;
    }

    public void TurnOn()
    {
        IsOn = true;
    }
}
