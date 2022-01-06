using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public class ColorHexProvider : RegexProviderBase
    {
        private static readonly Regex _regex = new(@"#([A-F0-9]{6}|[A-F0-9]{3})\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override Regex Regex => _regex;

        public override bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result)
        {
            Color color = ColorTranslator.FromHtml(match.Value);

            string newColor = direction == ShiftDirection.Up ? LightenBy(color, 5) : DarkenBy(color, 5);
            newColor = CorrectCasing(newColor, match.Value);

            result = new ShiftResult(match.Index, match.Length, newColor);

            return true;
        }

        private static string CorrectCasing(string newHex, string originalHex)
        {
            bool isUpper = originalHex.Any(h => char.IsUpper(h));

            if (!isUpper)
            {
                return newHex.ToLowerInvariant();
            }

            return newHex;
        }

        public static string LightenBy(Color color, int percent)
        {
            return ChangeColorBrightness(color, (float)(percent / 100.0));
        }

        public static string DarkenBy(Color color, int percent)
        {
            return ChangeColorBrightness(color, (float)(-1 * percent / 100.0));
        }

        // From: https://gist.github.com/zihotki/09fc41d52981fb6f93a81ebf20b35cd5
        private static string ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            Color newColor = Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
            string hex = HexConverter(newColor);

            return hex;
        }

        private static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
