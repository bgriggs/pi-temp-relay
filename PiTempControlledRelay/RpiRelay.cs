using System.Device.Gpio;

namespace PiTempControlledRelay;

internal class RpiRelay() : IRelayControl
{
    public bool IsOn { get; private set; }
    public int GpioPin { get; set; }

    public void TurnOff()
    {
        using GpioController controller = new();
        controller.OpenPin(GpioPin, PinMode.Output);
        controller.Write(GpioPin, PinValue.Low);
        IsOn = false;
    }

    public void TurnOn()
    {
        using GpioController controller = new();
        controller.OpenPin(GpioPin, PinMode.Output);
        controller.Write(GpioPin, PinValue.High);
        IsOn = true;
    }
}
