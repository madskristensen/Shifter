namespace Shifter.Providers
{
    public interface IProvider
    {
        bool TryShiftLine(string textIn, int caretPosition, ShiftDirection direction, out ShiftResult result);
        bool TryShiftSelection(string selectedText, ShiftDirection direction, out ShiftResult result);
    }
}