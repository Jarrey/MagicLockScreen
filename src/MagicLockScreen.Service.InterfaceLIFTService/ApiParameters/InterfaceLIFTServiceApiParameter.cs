using NoteOne_Core.Common;

namespace MagicLockScreen_Service_InterfaceLIFTService.ApiParameters
{
    public class InterfaceLIFTServiceApiParameter : ApiParameter
    {
        public InterfaceLIFTServiceApiParameter()
        {
            Parameters.Add(0, () => PageIndex);
        }

        public uint PageIndex { get; set; }
    }
}