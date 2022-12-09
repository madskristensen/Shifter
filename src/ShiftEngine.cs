using System.Collections.Generic;
using System.Linq;

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
            new BooleanProvider(),
        };

        public static bool TryShift(string textIn, int caretPosition, ShiftDirection direction, int? sequence, out ShiftResult result)
        {
            result = null;

            bool isIncremental = sequence.HasValue;
            if (isIncremental)
            {
                // Try shift only using incremental providers
                IEnumerable<IIncrementalProvider> incrementalProviders = _providers
                    .Select(static provider => provider as IIncrementalProvider)
                    .Where(static provider => provider != null);
                foreach (IIncrementalProvider incrementalProvider in incrementalProviders)
                {
                    if (incrementalProvider.TryShiftLine(textIn, caretPosition, direction, sequence.Value, out result))
                    {
                        return true;
                    }

                }
            }
            else
            {
                foreach (IProvider provider in _providers)
                {
                    if (provider.TryShiftLine(textIn, caretPosition, direction, out result))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
