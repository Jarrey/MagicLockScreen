using System;
using NoteOne_Core.Common;

namespace MagicLockScreen_Service_NationalGeographicService.ApiParameters
{
    public class NationalGeographicServiceApiParameter : ApiParameter
    {
        public NationalGeographicServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Date);
        }

        public DateTime Date { get; set; }
    }
}