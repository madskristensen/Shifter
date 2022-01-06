namespace Shifter.Providers
{
    public class ShiftResult
    {
        public ShiftResult(int start, int length, string shiftedText)
        {
            Start = start;
            Length = length;
            ShiftedText = shiftedText;
        }

        public int Start { get; }

        public int Length { get; }

        public string ShiftedText { get; }
    }
}
