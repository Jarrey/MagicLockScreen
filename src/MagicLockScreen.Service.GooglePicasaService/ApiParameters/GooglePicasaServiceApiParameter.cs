using NoteOne_Core.Common;

namespace MagicLockScreen_Service_GooglePicasaService.ApiParameters
{
    public class GooglePicasaServiceApiParameter : ApiParameter
    {
        public GooglePicasaServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              KeyWord);
            Parameters.Add(1, () =>
                              Count);
            Parameters.Add(2, () =>
                              StartIndex);
        }

        public string KeyWord { get; set; }
        public int Count { get; set; }
        public int StartIndex { get; set; }
    }
}