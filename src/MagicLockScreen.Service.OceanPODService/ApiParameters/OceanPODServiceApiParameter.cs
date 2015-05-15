using System;
using NoteOne_Core.Common;

namespace MagicLockScreen_Service_OceanPODService.ApiParameters
{
    public class OceanPODServiceApiParameter : ApiParameter
    {
        public OceanPODServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Date);
        }

        public DateTime Date { get; set; }
    }
}