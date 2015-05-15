using System;
using NoteOne_Core.Common;

namespace MagicLockScreen_Service_LunarPODService.ApiParameters
{
    public class LunarPODServiceApiParameter : ApiParameter
    {
        public LunarPODServiceApiParameter()
        {
            var datrTimeFormatMonthNames = new[]
                {
                    "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October"
                    , "November", "December"
                };
            Parameters.Add(0, () =>
                              datrTimeFormatMonthNames[Month]);
            Parameters.Add(1, () =>
                              Date);
        }

        public int Month { get; set; }
        public DateTime Date { get; set; }
    }
}