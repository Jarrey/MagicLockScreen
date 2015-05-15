using NoteOne_Core.Common;

namespace MagicLockScreen_Service_ImageSearchService.ApiParameters
{
    public class FlickrImageSearchServiceApiParameter : ApiParameter
    {
        public FlickrImageSearchServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Keyword);
            Parameters.Add(1, () =>
                              RecordPerPageNumber);
            Parameters.Add(2, () =>
                              PageNumber);
        }

        public string Keyword { get; set; }
        public int RecordPerPageNumber { get; set; }
        public int PageNumber { get; set; }
    }
}