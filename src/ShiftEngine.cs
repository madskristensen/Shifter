using System.Collections.Generic;

namespace Shifter.Providers
{
    public class ShiftEngine
    {
        private static readonly List<IProvider> _providers = new()
        {
            new ColorHexProvider(),
            new GuidProvider(),
            new NumberProvider(),
            new ColorNamesProvider(),
        };

        public static bool Parse(string textIn, int caretPosition, ShiftDirection direction, out ShiftResult result)
        {
            result = null;

            foreach (IProvider provider in _providers)
            {
                if (provider.TryShiftLine(textIn, caretPosition, direction, out result))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
