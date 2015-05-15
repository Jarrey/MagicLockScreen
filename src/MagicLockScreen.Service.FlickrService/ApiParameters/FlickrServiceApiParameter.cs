using NoteOne_Core.Common;

namespace MagicLockScreen_Service_FlickrService.ApiParameters
{
    public class FlickrServiceApiParameter : ApiParameter
    {
        public FlickrServiceApiParameter()
        {
            Parameters.Add(0, () =>
                              PerPage);
            Parameters.Add(1, () =>
                              Page);
        }

        public int PerPage { get; set; }
        public int Page { get; set; }
    }
}