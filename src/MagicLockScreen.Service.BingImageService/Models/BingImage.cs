using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_BingImageService.Models
{
    [DataContract]
    public class BingImage : BindableImage
    {
        #region Static Members

        public static string BaseUrl { get; set; }

        #endregion

        private string _copyright;

        private string _copyrightLink;

        private DateTime _date;

        [DataMember]
        public string Copyright
        {
            get { return _copyright; }
            set { SetProperty(ref _copyright, value); }
        }

        [DataMember]
        public string CopyrightLink
        {
            get { return _copyrightLink; }
            set { SetProperty(ref _copyrightLink, value); }
        }

        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }
    }
}