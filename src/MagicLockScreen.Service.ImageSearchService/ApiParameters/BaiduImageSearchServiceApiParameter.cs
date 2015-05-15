using NoteOne_Core.Common;

namespace MagicLockScreen_Service_ImageSearchService.ApiParameters
{
    public class BaiduImageSearchServiceApiParameter : ApiParameter
    {
        public BaiduImageSearchServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Keyword);
            Parameters.Add(1, () =>
                              RecordNumber);
            Parameters.Add(2, () =>
                              PageNumber);
        }

        public string Keyword { get; set; }
        public int RecordNumber { get; set; }
        public int PageNumber { get; set; }
    }
}