using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public class BooleanProvider : RegexProviderBase
    {
        private static readonly Regex _regex = new(@"[\w]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Dictionary<string, string> _terms = new()
        {
            { "true", "false" },
            { "yes", "no" },
            { "on", "off" },
            { "shown", "hidden" },
            { "show", "hide" },
            { "positive", "negative" },
            { "from", "until" },
            { "enable", "disable" },
            { "enabled", "disabled" },
            { "pass", "fail" },
            { "min", "max" },
            { "expand", "collapse" },
            { "asc", "desc" },
            { "first", "last" },
        };

        public override Regex Regex => _regex;

        public override bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result)
        {
            string term = match.Value.ToLowerInvariant();
            string opposite = _terms.FirstOrDefault(t => t.Key == term).Value ?? _terms.FirstOrDefault(x => x.Value == term).Key;

            string shiftedText = MatchCasing(opposite, match.Value);

            result = new ShiftResult(match.Index, match.Length, shiftedText);

            return true;
        }

        private static string MatchCasing(string newValue, string oldValue)
        {
            // Lower case
            if (oldValue.ToLowerInvariant() == oldValue)
            {
                return newValue;
            }

            // Lower case
            if (oldValue.ToUpperInvariant () == oldValue)
            {
                return newValue.ToUpperInvariant();
            }

            // Title case
            if (char.IsUpper(oldValue[0]))
                return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(newValue);

            return newValue;
        }
    }
}
