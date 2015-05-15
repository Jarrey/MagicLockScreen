using NoteOne.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MagicLockScreen.Service.AstronomyPictureService.Models
{
    [DataContract]
    public class AstronomyPicture : ModelBase
    {
        #region Static Members
        public static string BaseUrl { get; set; }
        #endregion

        private string _title;
        [DataMember]
        public string Title
        {
            get { return _title; }
            set
            {
                this.SetProperty(ref _title, value);
            }
        }

        private string _originalImageUrl;
        [DataMember]
        public string OriginalImageUrl
        {
            get { return _originalImageUrl; }
            set
            {
                this.SetProperty(ref _originalImageUrl, value);
            }
        }

        private string _thumbnailImageUrl;
        [DataMember]
        public string ThumbnailImageUrl
        {
            get { return _thumbnailImageUrl; }
            set
            {
                this.SetProperty(ref _thumbnailImageUrl, value);
            }
        }

        private string _copyright;
        [DataMember]
        public string Copyright
        {
            get { return _copyright; }
            set
            {
                this.SetProperty(ref _copyright, value);
            }
        }

        private string _explanation;
        [DataMember]
        public string Explanation
        {
            get { return _explanation; }
            set
            {
                this.SetProperty(ref _explanation, value);
            }
        }

        private DateTime _date;
        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                this.SetProperty(ref _date, value);
            }
        }

        public bool IsAvailable()
        {
            return (!string.IsNullOrEmpty(OriginalImageUrl) &&
                    !string.IsNullOrEmpty(ThumbnailImageUrl) &&
                    ThumbnailImageUrl != BaseUrl &&
                    OriginalImageUrl != BaseUrl);
        }
    }
}
