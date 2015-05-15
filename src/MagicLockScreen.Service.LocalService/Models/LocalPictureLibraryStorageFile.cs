using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace MagicLockScreen_Service_LocalService.Models
{
    [DataContract]
    public class LocalPictureLibraryStorageFile : LocalPictureLibraryStorageItem
    {
        public LocalPictureLibraryStorageFile(StorageFile storageFile, uint thumbnailSize = 60)
            : base(storageFile)
        {
            DisplayName = storageFile.DisplayName;
            ContentType = storageFile.ContentType;
            FileType = storageFile.FileType;
            StorageItem = storageFile;
            ThumbnailSize = thumbnailSize;
        }

        #region Properties

        private string _contentType;
        private string _displayName;

        private string _fileType;
        private string _size;
        private BitmapImage _thumbnailImage;
        private uint _thumbnailSize;
        private Size _imageSize;

        [DataMember]
        public string DisplayName
        {
            get { return _displayName; }
            private set { SetProperty(ref _displayName, value); }
        }

        [DataMember]
        public string FileType
        {
            get { return _fileType; }
            private set { SetProperty(ref _fileType, value); }
        }

        [DataMember]
        public string ContentType
        {
            get { return _contentType; }
            private set { SetProperty(ref _contentType, value); }
        }

        [DataMember]
        public string Size
        {
            get
            {
                CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
                if (coreWindow != null && coreWindow.Dispatcher.HasThreadAccess)
                {
                    SetFileSize();
                }
                return _size;
            }
            private set { SetProperty(ref _size, value); }
        }

        [DataMember]
        public uint ThumbnailSize
        {
            get { return _thumbnailSize; }
            private set { SetProperty(ref _thumbnailSize, value); }
        }

        [DataMember]
        public Size ImageSize
        {
            get
            {
                SetImageSize();
                return _imageSize;
            }
            private set { SetProperty(ref _imageSize, value); }
        }

        public BitmapImage ThumbnailImage
        {
            get
            {
                CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
                if (coreWindow != null && coreWindow.Dispatcher.HasThreadAccess)
                {
                    if (_thumbnailImage == null)
                    {
                        _thumbnailImage = new BitmapImage();
                        GnerateThumbnailImage(_thumbnailImage);
                    }
                }
                return _thumbnailImage;
            }
            private set { SetProperty(ref _thumbnailImage, value); }
        }

        public bool IsAvailableImageSize
        {
            get
            {
                var sizeSetting =
                    MagicLockScreen_Helper.AppSettings.Instance[
                        MagicLockScreen_Helper.AppSettings.LOCAL_IMAGE_SIZE_RANGE].ToString().StringToDictionary();

                double minWidth = sizeSetting["MinWidth"].StringToDouble();
                double minHeight = sizeSetting["MinHeight"].StringToDouble();
                double maxWidth = sizeSetting["MaxWidth"].StringToDouble();
                double maxHeight = sizeSetting["MaxHeight"].StringToDouble();

                return ImageSize.Width >= minWidth && ImageSize.Height >= minHeight &&
                       ImageSize.Width <= maxWidth && ImageSize.Height <= maxHeight;
            }
        }

        #endregion

        private async void GnerateThumbnailImage(BitmapImage thumbnailImage)
        {
            var file = StorageItem as StorageFile;
            if (file != null)
            {
                StorageItemThumbnail thumbnail =
                    await file.GetThumbnailAsync(ThumbnailMode.PicturesView, ThumbnailSize);
                await thumbnailImage.SetSourceAsync(thumbnail);
            }
        }

        private async void SetFileSize()
        {
            var file = StorageItem as StorageFile;
            if (file != null)
            {
                using (IRandomAccessStream stream = await (StorageItem as StorageFile).OpenAsync(FileAccessMode.Read))
                {
                    Size = stream.Size.ToFileSize();
                }
            }
        }

        private void SetImageSize()
        {
            var file = StorageItem as StorageFile;
            if (file != null)
            {
                ImageProperties imageProperties = null;

                Task<ImageProperties> t = file.Properties.GetImagePropertiesAsync().AsTask();
                t.Wait();
                if (t.Status == TaskStatus.RanToCompletion && t.Result != null)
                    imageProperties = t.Result;

                if (imageProperties != null)
                {
                    ImageSize = new Size(imageProperties.Width, imageProperties.Height);
                }
            }
        }
    }
}