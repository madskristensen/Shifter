using System.Globalization;
using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public class NumberProvider : RegexProviderBase
    {
        private static readonly Regex _regex = new(@"[-+]?\d*\.?\d+\b", RegexOptions.Compiled);

        public override Regex Regex => _regex;

        public override bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result)
        {
            result = null;

            if (!float.TryParse(match.Value, out float value))
            {
                return false;
            }

            int dot = match.Value.IndexOf('.');
            string format = dot > -1 ? "#.#0" : string.Empty;
            int decimalPlaces = NumberDecimalPlaces(match.Value);
            float delta = GetDelta(decimalPlaces);

            if (decimalPlaces > 0)
            {
                format = "F" + decimalPlaces;
            }

            string shiftedText;

            if (direction == ShiftDirection.Down)
            {
                shiftedText = (value - delta).ToString(format, CultureInfo.InvariantCulture);
            }
            else
            {
                shiftedText = (value + delta).ToString(format, CultureInfo.InvariantCulture);
            }

            if (dot == 0 && shiftedText.Length > 1)
            {
                shiftedText = shiftedText.Substring(1);
            }

            result = new ShiftResult(match.Index, match.Length, shiftedText);

            return true;
        }

        private static float GetDelta(int decimalPlaces)
        {
            return decimalPlaces switch
            {
                0 => 1F,
                1 => 0.1F,
                2 => 0.01F,
                3 => 0.001F,
                4 => 0.0001F,
                5 => 0.00001F,
                6 => 0.000001F,
                7 => 0.0000001F,
                8 => 0.00000001F,
                9 => 0.000000001F,
                _ => 0.0000000001F,
            };
        }

        private static int NumberDecimalPlaces(string value)
        {
            int s = value.IndexOf(".", StringComparison.CurrentCulture) + 1; // the first numbers plus decimal point
            if (s == 0)                     // No decimal point
            {
                return 0;
            }

            return value.Length - s;     //total length minus beginning numbers and decimal = number of decimal points
        }
    }
}
