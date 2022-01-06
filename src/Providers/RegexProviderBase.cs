using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public abstract class RegexProviderBase : IProvider
    {
        public abstract Regex Regex { get; }

        public virtual bool TryShiftSelection(string selectedText, ShiftDirection direction, out ShiftResult result)
        {
            int caretPosition = selectedText.Length;
            return TryShiftLine(selectedText, caretPosition, direction, out result);
        }

        public bool TryShiftLine(string textIn, int caretPosition, ShiftDirection direction, out ShiftResult result)
        {
            result = null;
            MatchCollection matches = Regex.Matches(textIn);

            if (matches.Count == 0)
            {
                return false;
            }

            foreach (Match match in matches)
            {
                int start = match.Index;
                int end = match.Index + match.Length;

                if (caretPosition >= start && caretPosition <= end)
                {
                    return TryShiftSelection(match, direction, out result);
                }
            }

            return false;
        }

        public abstract bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result);
    }
}
