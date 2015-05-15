using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_GooglePicasaService.Models
{
    [DataContract]
    public class GooglePicasa : BindableImage
    {
        #region Static Members

        #endregion

        #region URLs

        [DataMember] private string _ownerUrl;

        public string OwnerUrl
        {
            get { return _ownerUrl; }
            set { SetProperty(ref _ownerUrl, value); }
        }

        #endregion

        private string _id;

        private string _owner;
        private DateTime _published;
        private string _summary;

        private string _title;
        private DateTime _updated;

        [DataMember]
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        [DataMember]
        public string Owner
        {
            get { return _owner; }
            set { SetProperty(ref _owner, value); }
        }

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [DataMember]
        public string Summary
        {
            get { return _summary; }
            set { SetProperty(ref _summary, value); }
        }

        [DataMember]
        public DateTime Updated
        {
            get { return _updated; }
            set { SetProperty(ref _updated, value); }
        }

        [DataMember]
        public DateTime Published
        {
            get { return _published; }
            set { SetProperty(ref _published, value); }
        }
    }
}