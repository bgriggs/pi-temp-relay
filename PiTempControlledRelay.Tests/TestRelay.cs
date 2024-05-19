namespace PiTempControlledRelay.Tests;

internal class TestRelay : IRelayControl
{
    public int GpioPin { get; set; }

    public bool IsOn { get; private set; }

    public void TurnOff()
    {
        IsOn = false;
    }

    public void TurnOn()
    {
        IsOn = true;
    }
}
