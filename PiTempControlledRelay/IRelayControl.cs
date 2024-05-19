namespace PiTempControlledRelay;

public interface IRelayControl
{
    int GpioPin { get; }
    bool IsOn { get; }

    void InitializePin(int gpioPin);

    void TurnOn();
    void TurnOff();
}
