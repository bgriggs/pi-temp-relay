namespace PiTempControlledRelay;

public interface IRelayControl
{
    int GpioPin { get; set; }
    bool IsOn { get; }
    void TurnOn();
    void TurnOff();
}
