using System.Linq;
using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public class GuidProvider : RegexProviderBase
    {
        private static readonly Regex _regex = new(@"\b[0-9A-F]{8}(-[0-9A-F]{4}){3}-[0-9A-F]{12}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override Regex Regex => _regex;

        public override bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result)
        {
            result = null;

            if (!Guid.TryParse(match.Value, out Guid guid))
            {
                return false;
            }

            if (direction == ShiftDirection.Down)
            {
                result = new ShiftResult(match.Index, match.Length, PrevGuid(guid).ToString());
            }
            else
            {
                result = new ShiftResult(match.Index, match.Length, NextGuid(guid).ToString());
            }

            return true;
        }

        private static readonly int[] _byteOrder = { 15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3 };

        private static Guid NextGuid(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            bool canIncrement = _byteOrder.Any(i => ++bytes[i] != 0);
            return new Guid(canIncrement ? bytes : new byte[16]);
        }

        private static Guid PrevGuid(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            bool canIncrement = _byteOrder.Any(i => --bytes[i] != 0);
            return new Guid(canIncrement ? bytes : new byte[16]);
        }
    }
}
