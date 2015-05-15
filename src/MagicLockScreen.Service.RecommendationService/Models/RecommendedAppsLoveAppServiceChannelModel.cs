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
    public class RecommendedAppsLoveAppServiceChannelModel : RecommendedAppsServiceChannelModel
    {
        public RecommendedAppsLoveAppServiceChannelModel(ServiceChannel channel)
            : base(channel)
        {
            Logo = new Collection<BindableImage>
                {
                    new BindableImage
                        {
                            ThumbnailImageUrl =
                                @"ms-appx:///MagicLockScreen_Service_RecommendationService/Images/LoveAppBanner2x1.png",
                            IsThumbnailImageDownloading = false
                        }
                };

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
    }
}