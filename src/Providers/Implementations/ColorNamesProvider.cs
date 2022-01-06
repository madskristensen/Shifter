using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public class ColorNamesProvider : RegexProviderBase
    {
        private readonly List<string> _list = new()
        {
            "pink",
            "lightpink",
            "hotpink",
            "deeppink",
            "palevioletred",
            "mediumvioletred",
            "lavender",
            "thistle",
            "plum",
            "orchid",
            "violet",
            "fuchsia",
            "magenta",
            "mediumorchid",
            "darkorchid",
            "darkviolet",
            "blueviolet",
            "darkmagenta",
            "purple",
            "mediumpurple",
            "mediumslateblue",
            "slateblue",
            "darkslateblue",
            "rebeccapurple",
            "indigo ",
            "lightsalmon",
            "salmon",
            "darksalmon",
            "lightcoral",
            "indianred ",
            "crimson",
            "red",
            "firebrick",
            "darkred",
            "orange",
            "darkorange",
            "coral",
            "tomato",
            "orangered",
            "gold",
            "yellow",
            "lightyellow",
            "lemonchiffon",
            "lightgoldenrodyellow",
            "papayawhip",
            "moccasin",
            "peachpuff",
            "palegoldenrod",
            "khaki",
            "darkkhaki",
            "greenyellow",
            "chartreuse",
            "lawngreen",
            "lime",
            "limegreen",
            "palegreen",
            "lightgreen",
            "mediumspringgreen",
            "springgreen",
            "mediumseagreen",
            "seagreen",
            "forestgreen",
            "green",
            "darkgreen",
            "yellowgreen",
            "olivedrab",
            "darkolivegreen",
            "mediumaquamarine",
            "darkseagreen",
            "lightseagreen",
            "darkcyan",
            "teal",
            "aqua",
            "cyan",
            "lightcyan",
            "paleturquoise",
            "aquamarine",
            "turquoise",
            "mediumturquoise",
            "darkturquoise",
            "cadetblue",
            "steelblue",
            "lightsteelblue",
            "lightblue",
            "powderblue",
            "lightskyblue",
            "skyblue",
            "cornflowerblue",
            "deepskyblue",
            "dodgerblue",
            "royalblue",
            "blue",
            "mediumblue",
            "darkblue",
            "navy",
            "midnightblue",
            "cornsilk",
            "blanchedalmond",
            "bisque",
            "navajowhite",
            "wheat",
            "burlywood",
            "tan",
            "rosybrown",
            "sandybrown",
            "goldenrod",
            "darkgoldenrod",
            "peru",
            "chocolate",
            "olive",
            "saddlebrown",
            "sienna",
            "brown",
            "maroon",
            "white",
            "snow",
            "honeydew",
            "mintcream",
            "azure",
            "aliceblue",
            "ghostwhite",
            "whitesmoke",
            "seashell",
            "beige",
            "oldlace",
            "floralwhite",
            "ivory",
            "antiquewhite",
            "linen",
            "lavenderblush",
            "mistyrose",
            "gainsboro",
            "lightgray",
            "silver",
            "darkgray",
            "dimgray",
            "gray",
            "lightslategray",
            "slategray",
            "darkslategray",
            "black",

        };

        private static readonly Regex _regex = new(@"\b[a-zA-Z]+\b", RegexOptions.Compiled);
        public override Regex Regex => _regex;

        public override bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result)
        {
            result = null;

            int index = _list.IndexOf(match.Value.ToLowerInvariant());

            if (index > -1 && index < _list.Count - 1)
            {
                string shiftedText = direction == ShiftDirection.Down ? _list[index + 1] : _list[index - 1];

                if (char.IsUpper(match.Value[0]))
                {
                    shiftedText = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(shiftedText);
                }

                result = new(match.Index, match.Length, shiftedText);
                return true;
            }

            return false;
        }
    }
}
