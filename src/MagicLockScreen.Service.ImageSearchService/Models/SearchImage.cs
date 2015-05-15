using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_ImageSearchService.Models
{
    [DataContract]
    public class SearchImage : BindableImage
    {
        private string _content;
        private string _copyright;
        private string _copyrightUrl;
        private DateTime _date;
        private string _pageUrl;
        private string _title;

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [DataMember]
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        [DataMember]
        public string Copyright
        {
            get { return _copyright; }
            set { SetProperty(ref _copyright, value); }
        }

        [DataMember]
        public string CopyrightUrl
        {
            get { return _copyrightUrl; }
            set { SetProperty(ref _copyrightUrl, value); }
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
    }
}