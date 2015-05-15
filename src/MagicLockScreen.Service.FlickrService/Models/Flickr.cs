using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_FlickrService.Models
{
    [DataContract]
    public class Flickr : BindableImage
    {
        #region Static Members

        public static string BaseImageUrl { get; set; }
        public static string BaseUserProfileUrl { get; set; }
        public static string BasePhotoProfileUrl { get; set; }

        #endregion

        #region URLs

        [DataMember]
        public string UserProfileUrl
        {
            get { return string.Format(BaseUserProfileUrl, Owner); }
        }

        [DataMember]
        public string PhotoProfileUrl
        {
            get { return string.Format(BasePhotoProfileUrl, Owner, Id); }
        }

        public void SetImageUrl()
        {
            ThumbnailImageUrl = string.Format(BaseImageUrl, Farm, Server, Id, Secret, "");
        }

        public void SetBigImageUrl()
        {
            OriginalImageUrl = string.Format(BaseImageUrl, Farm, Server, Id, Secret, "_b");
        }

        #endregion

        private DateTime _dateUpload;
        private string _description;
        private string _farm;
        private string _id;

        private string _owner;

        private string _ownerName;

        private string _secret;

        private string _server;
        private string _title;

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
        public string OwnerName
        {
            get { return _ownerName; }
            set { SetProperty(ref _ownerName, value); }
        }

        [DataMember]
        public string Secret
        {
            get { return _secret; }
            set { SetProperty(ref _secret, value); }
        }

        [DataMember]
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }

        [DataMember]
        public string Farm
        {
            get { return _farm; }
            set { SetProperty(ref _farm, value); }
        }

        [DataMember]
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [DataMember]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        [DataMember]
        public DateTime DateUpload
        {
            get { return _dateUpload; }
            set { SetProperty(ref _dateUpload, value); }
        }
    }
}