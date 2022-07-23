using QuickSockets.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSockets.Validation
{
    internal static class OptionValidation
    {
        internal static bool EssentialOptionsAreValid(EssentialOptions? essentials)
        {
            if (essentials == null)
                throw new ArgumentNullException(nameof(essentials));

            if (essentials.DeviceIdentifier.Contains(Constants.ConfigurationConstants.DATA_DIVIDER))
                throw new Exception("Device Identifier cannot contain the character-sequence '" + Constants.ConfigurationConstants.DATA_DIVIDER + "'");
            
            // TODO: Implement
            return true;
        }
    }
}
