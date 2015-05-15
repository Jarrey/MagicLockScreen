using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Models;
using MagicLockScreen_Service_NationalGeographicService.Resources;
using MagicLockScreen_Service_NationalGeographicService.UI.Views;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.UI.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace MagicLockScreen_Service_NationalGeographicService.Models
{
    public class NationalGeographicServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public NationalGeographicServiceChannelModel(ServiceChannel channel)
            : base(channel)
        {
            Index = 6;
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
            PrimaryViewType = typeof (NationalGeographicServiceChannelPage);
        }

        public override bool IsEnabled
        {
            get { return (bool) AppSettings.Instance[AppSettings.SERVICE_CHANNEL_STATUS_NGSC]; }
            set
            {
                base.IsEnabled = value;
                AppSettings.Instance[AppSettings.SERVICE_CHANNEL_STATUS_NGSC] = value;
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