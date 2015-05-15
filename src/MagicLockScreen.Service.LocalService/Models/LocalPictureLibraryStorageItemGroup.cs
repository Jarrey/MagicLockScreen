using System.Collections.ObjectModel;
using Windows.Storage;

namespace MagicLockScreen_Service_LocalService.Models
{
    public class LocalPictureLibraryStorageItemGroup : ObservableCollection<LocalPictureLibraryStorageItemCollection>
    {
        public LocalPictureLibraryStorageItemCollection this[StorageItemTypes type]
        {
            get
            {
                LocalPictureLibraryStorageItemCollection itemCollection = null;
                foreach (LocalPictureLibraryStorageItemCollection item in this)
                {
                    if (item.Type == type)
                        itemCollection = item;
                }
                return itemCollection;
            }
        }

        public LocalPictureLibraryStorageItemCollection Add(StorageItemTypes type)
        {
            if (this[type] == null)
            {
                var itemCollection = new LocalPictureLibraryStorageItemCollection(type);
                Add(itemCollection);
                return itemCollection;
            }
            else
                return this[type];
        }
    }
}