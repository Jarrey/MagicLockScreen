using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_OceanPODService.Models
{
    [DataContract]
    public class OceanPOD : BindableImage
    {
        #region Static Members

        public static string Url { get; set; }

        #endregion

        private DateTime _date;
        private string _explanation;
        private string _pageUrl;
        private string _title;

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
            get { return !string.IsNullOrEmpty(OriginalImageUrl); }
        }
    }
}