using NoteOne_Core.Common;

namespace MagicLockScreen_Service_ImageSearchService.ApiParameters
{
    public class InfoImageSearchServiceApiParameter : ApiParameter
    {
        public InfoImageSearchServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              PageNumber);
            Parameters.Add(1, () =>
                              Keyword);
        }

        public int PageNumber { get; set; }
        public string Keyword { get; set; }
    }
}