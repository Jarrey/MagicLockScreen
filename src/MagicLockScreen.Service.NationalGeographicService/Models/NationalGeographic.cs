using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_NationalGeographicService.Models
{
    [DataContract]
    public class NationalGeographic : BindableImage
    {
        #region Static Members

        public static string BaseUrl { get; set; }

        #endregion

        private string _copyright;
        private DateTime _date;
        private string _explanation;
        private string _previousPhotoLink;
        private string _title;
        private string _url;

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [DataMember]
        public string Explanation
        {
            get { return _explanation; }
            set { SetProperty(ref _explanation, value); }
        }

        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        [DataMember]
        public string PreviousPhotoLink
        {
            get { return _previousPhotoLink; }
            set { SetProperty(ref _previousPhotoLink, value); }
        }

        [DataMember]
        public string Copyright
        {
            get { return _copyright; }
            set { SetProperty(ref _copyright, value); }
        }

        [DataMember]
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }
    }
}