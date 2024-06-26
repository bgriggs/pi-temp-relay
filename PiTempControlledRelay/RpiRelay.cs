﻿using System.Device.Gpio;

namespace PiTempControlledRelay;

internal class RpiRelay : IRelayControl
{
    public bool IsOn { get; private set; }
    public int GpioPin { get; private set; }
    private readonly GpioController controller = new();

    public void InitializePin(int gpioPin)
    {
        GpioPin = gpioPin;
        controller.OpenPin(GpioPin, PinMode.Output);
    }

    public void TurnOff()
    {
        controller.Write(GpioPin, PinValue.High);
        IsOn = false;
    }

    public void TurnOn()
    {
        controller.Write(GpioPin, PinValue.Low);
        IsOn = true;
    }
}
