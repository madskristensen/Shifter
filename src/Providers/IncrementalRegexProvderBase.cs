using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public abstract class IncrementalRegexProviderBase : RegexProviderBase, IIncrementalProvider
    {
        /// <inheritdoc />
        public bool TryShiftLine(string textIn, int caretPosition, ShiftDirection direction, int sequence, out ShiftResult result)
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
                    return TryShiftSelection(match, direction, sequence, out result);
                }
            }

            return false;
        }

        public override bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result) => TryShiftSelection(match, direction, 0, out result);

        public abstract bool TryShiftSelection(Match match, ShiftDirection direction, int sequence, out ShiftResult result);
    }
}