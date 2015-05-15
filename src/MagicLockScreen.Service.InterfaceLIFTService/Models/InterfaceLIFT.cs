using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_InterfaceLIFTService.Models
{
    [DataContract]
    public class InterfaceLIFT : BindableImage
    {
        #region Static Members

        public static string BaseUrl { get; set; }
        public static string Url { get; set; }

        #endregion

        private string _authorUrl;
        private string _copyright;
        private string _explanation;
        private int _imageId;
        private string _pageUrl;

        private string _title;

        [DataMember]
        public int ImageId
        {
            get { return _imageId; }
            set { SetProperty(ref _imageId, value); }
        }

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [DataMember]
        public string Copyright
        {
            get { return _copyright; }
            set { SetProperty(ref _copyright, value); }
        }

        [DataMember]
        public string Explanation
        {
            get { return _explanation; }
            set { SetProperty(ref _explanation, value); }
        }

        [DataMember]
        public string PageUrl
        {
            get { return _pageUrl; }
            set { SetProperty(ref _pageUrl, value); }
        }

        [DataMember]
        public string AuthorUrl
        {
            get { return _authorUrl; }
            set { SetProperty(ref _authorUrl, value); }
        }

        public bool IsAvailable
        {
            get
            {
                return (!string.IsNullOrEmpty(OriginalImageUrl) &&
                        !string.IsNullOrEmpty(ThumbnailImageUrl) &&
                        ThumbnailImageUrl != BaseUrl &&
                        OriginalImageUrl != BaseUrl);
            }
        }
    }
}