using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_WWFPictureService.Models
{
    [DataContract]
    public class WWFPicture : BindableImage
    {
        #region Static Members

        public static string BaseUrl { get; set; }

        #endregion

        private string _copyright;
        private string _explanation;
        private string _pageUrl;
        private DateTime _date;
        private string _title;

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
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public bool IsAvailable
        {
            get
            {
                return (!string.IsNullOrEmpty(OriginalImageUrl) &&
                        !string.IsNullOrEmpty(ThumbnailImageUrl));
            }
        }
    }
}