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
    public class RecommendedAppsServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public RecommendedAppsServiceChannelModel(ServiceChannel channel)
            : base(channel)
        {
            Index = 10;
            Title = string.Empty;
            SubTitle = string.Empty;
            GroupID = ServiceChannelGroupID.RecommendedApps;
            ShowOverlay = false;
            PrimaryViewType = null;
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
                        if (Channel is RecommendedAppsServiceChannel)
                        {
                            var uri = new Uri((Channel as RecommendedAppsServiceChannel).URL);
                            await Launcher.LaunchUriAsync(uri);
                        }
                    });
            }
        }

        #endregion
    }
}