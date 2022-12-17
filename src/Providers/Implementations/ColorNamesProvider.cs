using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shifter.Providers
{
    public class ColorNamesProvider : RegexProviderBase
    {
        private static readonly List<string> _list = new()
        {
            "Pink",
            "LightPink",
            "HotPink",
            "DeepPink",
            "PaleVioletRed",
            "MediumVioletRed",
            "Lavender",
            "Thistle",
            "Plum",
            "Orchid",
            "Violet",
            "Fuchsia",
            "Magenta",
            "MediumOrchid",
            "DarkOrchid",
            "DarkViolet",
            "BlueViolet",
            "DarkMagenta",
            "Purple",
            "MediumPurple",
            "MediumSlateBlue",
            "SlateBlue",
            "DarkSlateBlue",
            "RebeccaPurple",
            "Indigo",
            "LightSalmon",
            "Salmon",
            "DarkSalmon",
            "LightCoral",
            "IndianRed",
            "Crimson",
            "Red",
            "Firebrick",
            "DarkRed",
            "Orange",
            "DarkOrange",
            "Coral",
            "Tomato",
            "OrangeRed",
            "Gold",
            "Yellow",
            "LightYellow",
            "LemonChiffon",
            "LightGoldenrodYellow",
            "PapayaWhip",
            "Moccasin",
            "PeachPuff",
            "PaleGoldenrod",
            "Khaki",
            "DarkKhaki",
            "GreenYellow",
            "Chartreuse",
            "LawnGreen",
            "Lime",
            "LimeGreen",
            "PaleGreen",
            "LightGreen",
            "MediumSpringGreen",
            "SpringGreen",
            "MediumSeaGreen",
            "SeaGreen",
            "ForestGreen",
            "Green",
            "DarkGreen",
            "YellowGreen",
            "OliveDrab",
            "DarkOliveGreen",
            "MediumAquamarine",
            "DarkSeaGreen",
            "LightSeaGreen",
            "DarkCyan",
            "Teal",
            "Aqua",
            "Cyan",
            "LightCyan",
            "PaleTurquoise",
            "Aquamarine",
            "Turquoise",
            "MediumTurquoise",
            "DarkTurquoise",
            "CadetBlue",
            "SteelBlue",
            "LightSteelBlue",
            "LightBlue",
            "PowderBlue",
            "LightSkyBlue",
            "SkyBlue",
            "CornflowerBlue",
            "DeepSkyBlue",
            "DodgerBlue",
            "RoyalBlue",
            "Blue",
            "MediumBlue",
            "DarkBlue",
            "Navy",
            "MidnightBlue",
            "Cornsilk",
            "BlanchedAlmond",
            "Bisque",
            "NavajoWhite",
            "Wheat",
            "BurlyWood",
            "Tan",
            "RosyBrown",
            "SandyBrown",
            "Goldenrod",
            "DarkGoldenrod",
            "Peru",
            "Chocolate",
            "Olive",
            "SaddleBrown",
            "Sienna",
            "Brown",
            "Maroon",
            "White",
            "Snow",
            "Honeydew",
            "MintCream",
            "Azure",
            "AliceBlue",
            "GhostWhite",
            "WhiteSmoke",
            "SeaShell",
            "Beige",
            "OldLace",
            "FloralWhite",
            "Ivory",
            "AntiqueWhite",
            "Linen",
            "LavenderBlush",
            "MistyRose",
            "Gainsboro",
            "LightGray",
            "Silver",
            "DarkGray",
            "DimGray",
            "Gray",
            "LightSlateGray",
            "SlateGray",
            "DarkSlateGray",
            "Black",

        };

        private static readonly List<string> _listLower;

        static ColorNamesProvider()
        {
            _listLower = _list.Select(s => s.ToLowerInvariant()).ToList();
        }

        private static readonly Regex _regex = new(@"\b[a-zA-Z]+\b", RegexOptions.Compiled);
        public override Regex Regex => _regex;

        public override bool TryShiftSelection(Match match, ShiftDirection direction, out ShiftResult result)
        {
            result = null;

            int index = _listLower.IndexOf(match.Value.ToLowerInvariant());

            if (index < 0 || index >= _list.Count) return false;

            // Get index with allowed rotation
            index = GetShiftedIndex(index, direction == ShiftDirection.Down);
            bool isUpper = char.IsUpper(match.Value[0]);

            // If first letter was upper, assume the color should be from PascalCase list else use lowercase
            string shiftedText = isUpper 
                ? _list[index] 
                : _listLower[index];

            result = new(match.Index, match.Length, shiftedText);
            return true;

            static int GetShiftedIndex(int index, bool down) => (index + _list.Count + (down ? 1 : -1)) % _list.Count;
        }
    }
}
