using System;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Models;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using Windows.System;

namespace MagicLockScreen_Service_RecommendationService.Models
{
    public class RecommendedAppsLoveAppServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public RecommendedAppsLoveAppServiceChannelModel(ServiceChannel channel)
            : base(channel)
        {
            Index = 10;
            Title = string.Empty;
            SubTitle = string.Empty;
            Logo = new Collection<BindableImage>
                {
                    new BindableImage
                        {
                            ThumbnailImageUrl =
                                @"ms-appx:///MagicLockScreen_Service_RecommendationService/Images/LoveAppBanner2x1.png",
                            IsThumbnailImageDownloading = false
                        }
                };
            GroupID = ServiceChannelGroupID.RecommendedApps;
            ShowOverlay = false;
            PrimaryViewType = null;

            // second logo
            HasSecondLogo = true;
            SecondLogo = new Collection<BindableImage>
                {
                    new BindableImage
                        {
                            ThumbnailImageUrl =
                                @"ms-appx:///MagicLockScreen_Service_RecommendationService/Images/LoveAppBanner2x2.png",
                            IsThumbnailImageDownloading = false
                        }
                };
        }

        public override bool IsEnabled
        {
            get { return (bool) AppSettings.Instance[AppSettings.SERVICE_CHANNEL_STATUS_RALASC]; }
            set
            {
                base.IsEnabled = value;
                AppSettings.Instance[AppSettings.SERVICE_CHANNEL_STATUS_RALASC] = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        #region Commands

        public RelayCommand<object[]> ItemClickCommand
        {
            get
            {
                return new RelayCommand<object[]>(async p =>
                    {
                        if (Channel is RecommendedAppsLoveAppServiceChannel)
                        {
                            var uri = new Uri((Channel as RecommendedAppsLoveAppServiceChannel).URL);
                            await Launcher.LaunchUriAsync(uri);
                        }
                    });
            }
        }

        #endregion
    }
}