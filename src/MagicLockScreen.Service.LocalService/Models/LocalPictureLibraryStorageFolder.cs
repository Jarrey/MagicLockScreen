using System.Runtime.Serialization;
using Windows.Storage;

namespace MagicLockScreen_Service_LocalService.Models
{
    [DataContract]
    public class LocalPictureLibraryStorageFolder : LocalPictureLibraryStorageItem
    {
        private string _displayName;

        private string _parentPath;

        public LocalPictureLibraryStorageFolder(StorageFolder storageFolder, string parentPath)
            : base(storageFolder)
        {
            DisplayName = storageFolder.DisplayName;
            StorageItem = storageFolder;
            ParentPath = parentPath;
        }

        [DataMember]
        public string DisplayName
        {
            get { return _displayName; }
            private set { SetProperty(ref _displayName, value); }
        }

        [DataMember]
        public string ParentPath
        {
            get { return _parentPath; }
            private set { SetProperty(ref _parentPath, value); }
        }
    }
}