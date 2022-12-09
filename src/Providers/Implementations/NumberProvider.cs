using System.Globalization;
using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public class NumberProvider : IncrementalRegexProviderBase
    {
        private static readonly Regex _regex = new(@"[-+]?\d*\.?\d+\b", RegexOptions.Compiled);

        public override Regex Regex => _regex;

        public override bool TryShiftSelection(Match match, ShiftDirection direction, int sequence, out ShiftResult result)
        {
            result = null;

            if (!float.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
            {
                return false;
            }

            int dot = match.Value.IndexOf('.');
            string format = dot > -1 ? "#.#0" : string.Empty;
            int decimalPlaces = NumberDecimalPlaces(match.Value);
            float delta = GetDelta(decimalPlaces, sequence);

            if (decimalPlaces > 0)
            {
                format = "F" + decimalPlaces;
            }

            float shiftedValue = direction == ShiftDirection.Down
                ? value - delta
                : value + delta;

            string shiftedText = shiftedValue.ToString(format, CultureInfo.InvariantCulture); ;
            if (dot == 0 && shiftedText.Length > 1)
            {
                shiftedText = shiftedText.Substring(1);
            }

            // Keep leading zeros
            if (KeepLeadingZeros(match.Value, value))
            {
                int padLength = GetPadLength(match.Value, value, shiftedValue);
                shiftedText = shiftedText.PadLeft(padLength, '0');

                if (shiftedValue < 0)
                {
                    shiftedText = $"-{shiftedText.Replace("-", "")}";
                }
            }

            result = new ShiftResult(match.Index, match.Length, shiftedText);
            return true;

            static bool KeepLeadingZeros(string value, float numValue)
            {
                // 0.1
                if (value[0] == '0' && value.Length > 1 && value[1] == '.') return false;
                // -0.1
                if (value[0] == '-' && value.Length > 2 && value[1] == '0' && value[2] == '.') return false;
                // 0123
                if (value[0] == '0' && numValue != 0) return true;
                // -001
                if (value[0] == '-' && value.Length > 1 && value[1] == '0') return true;
                // 0000
                if (value.Length > 1 && numValue == 0 && !value.Contains(".")) return true;

                return false;
            }

            static int GetPadLength(string textValue, float oldValue, float newValue)
            {
                int textLength = textValue.Length;
                bool oldNegative = oldValue < 0;
                bool newNegative = newValue < 0;
                return (oldNegative, newNegative) switch
                {
                    (true, true) => textLength,
                    (true, false) => textLength - 1,
                    (false, true) => textLength + 1,
                    (false, false) => textLength,
                };
            }
        }

        private static float GetDelta(int decimalPlaces, int sequence)
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
            } * (1 + sequence);
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
