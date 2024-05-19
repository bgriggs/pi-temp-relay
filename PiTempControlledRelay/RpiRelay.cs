using System.Device.Gpio;

namespace PiTempControlledRelay;

internal class RpiRelay : IRelayControl
{
    public bool IsOn { get; private set; }
    public int GpioPin { get; private set; }
    private GpioController controller = new();

    public void InitializePin(int gpioPin)
    {
        GpioPin = gpioPin;
        controller.OpenPin(GpioPin, PinMode.Output);
    }

    public void TurnOff()
    {
        controller.Write(GpioPin, PinValue.Low);
        IsOn = false;
    }

    public void TurnOn()
    {
        controller.Write(GpioPin, PinValue.High);
        IsOn = true;
    }
}
