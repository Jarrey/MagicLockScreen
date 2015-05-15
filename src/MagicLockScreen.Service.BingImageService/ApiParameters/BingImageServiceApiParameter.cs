using NoteOne_Core.Common;

namespace MagicLockScreen_Service_BingImageService.ApiParameters
{
    public class BingImageServiceApiParameter : ApiParameter
    {
        public BingImageServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Index);
            Parameters.Add(1, () =>
                              Number);
            Parameters.Add(2, () =>
                              Market);
        }

        public int Index { get; set; }
        public int Number { get; set; }
        public string Market { get; set; }
    }
}