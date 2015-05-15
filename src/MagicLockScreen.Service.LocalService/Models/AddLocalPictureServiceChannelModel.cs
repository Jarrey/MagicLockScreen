using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Models;
using MagicLockScreen_Service_LocalService.UI.Views;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Service_LocalService.Models
{
    public class AddLocalPictureServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public AddLocalPictureServiceChannelModel(ServiceChannel channel)
            : base(channel)
        {
            Index = 1000;
            Title = ConstKeys.ADD_SERVICE_CHANNEL_MODEL_KEY;
            SubTitle = string.Empty;
            Logo = new Collection<BindableImage>
                {
                    new BindableImage
                        {
                            ThumbnailImageUrl = @"ms-appx:///Assets/Add.png",
                            IsThumbnailImageDownloading = false
                        }
                };
            ShowOverlay = false;
            GroupID = ServiceChannelGroupID.LocalPictures;
            PrimaryViewType = typeof (LocalPictureFolderSelectionPage);
        }

        public override bool IsEnabled
        {
            get { return true; }
        }

        #region Commands

        #endregion
    }
}