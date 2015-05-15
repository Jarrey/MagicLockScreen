using System.Runtime.Serialization;

namespace MagicLockScreen_Service_ImageSearchService.Models
{
    [DataContract]
    public class FlickrSearchImage : SearchImage
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

        private string _farm;
        private string _id;
        private string _owner;
        private string _secret;
        private string _server;

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
    }
}