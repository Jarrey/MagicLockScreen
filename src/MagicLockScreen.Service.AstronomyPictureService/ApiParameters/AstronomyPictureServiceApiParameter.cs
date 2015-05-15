using System;
using NoteOne_Core.Common;

namespace MagicLockScreen_Service_AstronomyPictureService.ApiParameters
{
    public class AstronomyPictureServiceApiParameter : ApiParameter
    {
        public AstronomyPictureServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Date);
        }

        public DateTime Date { get; set; }
    }
}