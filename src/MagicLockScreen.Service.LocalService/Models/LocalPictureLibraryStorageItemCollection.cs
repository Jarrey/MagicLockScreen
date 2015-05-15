using System.Collections.ObjectModel;
using Windows.Storage;

namespace MagicLockScreen_Service_LocalService.Models
{
    public class LocalPictureLibraryStorageItemCollection : ObservableCollection<LocalPictureLibraryStorageItem>
    {
        public LocalPictureLibraryStorageItemCollection(StorageItemTypes type)
        {
            Type = type;
        }

        #region Properties

        public StorageItemTypes Type { get; private set; }

        #endregion
    }
}