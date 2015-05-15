using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Models;
using MagicLockScreen_Service_WWFPictureService.Resources;
using MagicLockScreen_Service_WWFPictureService.UI.Views;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.UI.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace MagicLockScreen_Service_WWFPictureService.Models
{
    public class WWFPictureServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public WWFPictureServiceChannelModel(ServiceChannel channel)
            : base(channel)
        {
            Index = 10;
            Title = ResourcesLoader.Loader["ServiceChannelTitle"];
            SubTitle = ResourcesLoader.Loader["ServiceChannelSubTitle"];
            Logo = new Collection<BindableImage>
                {
                    new BindableImage
                        {
                            ThumbnailImageUrl = @"ms-appx:///Assets/ServiceChannelLogo//ChannelLogo2x2.png",
                            IsThumbnailImageDownloading = false
                        }
                };
            GroupID = ServiceChannelGroupID.OnlinePictures;
            PrimaryViewType = typeof (WWFPictureServiceChannelPage);
        }

        public override bool IsEnabled
        {
            get { return (bool) AppSettings.Instance[AppSettings.SERVICE_CHANNEL_STATUS_ILS]; }
            set
            {
                base.IsEnabled = value;
                AppSettings.Instance[AppSettings.SERVICE_CHANNEL_STATUS_ILS] = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public RelayCommand ShowInfoCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        var information =
                            XamlReader.Load(ResourcesLoader.Loader["ServiceChannelInformation"]) as UIElement;
                        if (information != null)
                        {
                            var settingPopup = new SettingPopup(ResourcesLoader.Loader["InformationTitle"])
                                {
                                    Content = information
                                };
                            settingPopup.Show();
                        }
                    });
            }
        }
    }
}