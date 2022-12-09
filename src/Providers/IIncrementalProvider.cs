namespace Shifter.Providers
{
    public interface IIncrementalProvider : IProvider
    {
        bool TryShiftLine(string textIn, int caretPosition, ShiftDirection direction, int sequence, out ShiftResult result);
    }
}