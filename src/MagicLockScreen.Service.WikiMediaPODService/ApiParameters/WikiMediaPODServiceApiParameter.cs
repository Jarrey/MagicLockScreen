using System;
using NoteOne_Core.Common;

namespace MagicLockScreen_Service_WikiMediaPODService.ApiParameters
{
    public class WikiMediaPODServiceApiParameter : ApiParameter
    {
        public WikiMediaPODServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Date);
        }

        public DateTime Date { get; set; }
    }
}