using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_LocalService.Models
{
    [DataContract]
    public class LocalPictureLibrary : BindableImage
    {
        #region Static Members

        #endregion

        private DateTime _dateCreated;
        private string _explanation;
        private string _title;

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [DataMember]
        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { SetProperty(ref _dateCreated, value); }
        }

        [DataMember]
        public string Explanation
        {
            get { return _explanation; }
            set { SetProperty(ref _explanation, value); }
        }
    }
}