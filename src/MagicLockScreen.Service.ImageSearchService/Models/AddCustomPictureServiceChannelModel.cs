using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Models;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using Windows.ApplicationModel.Search;
using Windows.UI.ViewManagement;

namespace MagicLockScreen_Service_ImageSearchService.Models
{
    public class AddCustomPictureServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public AddCustomPictureServiceChannelModel(ServiceChannel channel)
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
            GroupID = ServiceChannelGroupID.CustomSearchPictures;
            PrimaryViewType = null;
        }

        public override bool IsEnabled
        {
            get { return true; }
        }

        #region Commands

        public RelayCommand<object[]> ItemClickCommand
        {
            get
            {
                return new RelayCommand<object[]>(async p =>
                    {
                        await Task.Delay(100);
                        SearchPane searchPane = SearchPane.GetForCurrentView();
                        searchPane.Show();
                    });
            }
        }

        #endregion
    }
}