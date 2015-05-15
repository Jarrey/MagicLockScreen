using NoteOne_Core.Common;

namespace MagicLockScreen_Service_WWFPictureService.ApiParameters
{
    public class WWFPictureServiceApiParameter : ApiParameter
    {
        public WWFPictureServiceApiParameter()
        {
            Parameters.Add(0, () => PageIndex);
        }

        public uint PageIndex { get; set; }
    }
}