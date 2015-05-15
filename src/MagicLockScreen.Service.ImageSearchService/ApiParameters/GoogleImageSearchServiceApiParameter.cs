using NoteOne_Core.Common;

namespace MagicLockScreen_Service_ImageSearchService.ApiParameters
{
    public class GoogleImageSearchServiceApiParameter : ApiParameter
    {
        public GoogleImageSearchServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              Keyword);
            Parameters.Add(1, () =>
                              RecordNumber);
            Parameters.Add(2, () =>
                              StartIndex);
        }

        public string Keyword { get; set; }
        public int RecordNumber { get; set; }
        public int StartIndex { get; set; }
    }
}