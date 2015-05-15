using System;
using System.Runtime.Serialization;
using NoteOne_Core.Common;
using Windows.Storage;

namespace MagicLockScreen_Service_LocalService.Models
{
    [DataContract]
    public class LocalPictureLibraryStorageItem : ModelBase
    {
        private DateTime _dateCreated;
        private string _name;
        private string _path;
        private StorageItemTypes _type;

        public LocalPictureLibraryStorageItem(IStorageItem storageItem)
        {
            Name = storageItem.Name;
            DateCreated = storageItem.DateCreated.DateTime;
            Path = storageItem.Path;
            Type = storageItem.IsOfType(StorageItemTypes.Folder) ? StorageItemTypes.Folder : StorageItemTypes.File;
        }

        public IStorageItem StorageItem { get; protected set; }

        [DataMember]
        public string Name
        {
            get { return _name; }
            private set { SetProperty(ref _name, value); }
        }

        [DataMember]
        public StorageItemTypes Type
        {
            get { return _type; }
            private set { SetProperty(ref _type, value); }
        }

        [DataMember]
        public DateTime DateCreated
        {
            get { return _dateCreated; }
            private set { SetProperty(ref _dateCreated, value); }
        }

        [DataMember]
        public string Path
        {
            get { return _path; }
            private set { SetProperty(ref _path, value); }
        }
    }
}