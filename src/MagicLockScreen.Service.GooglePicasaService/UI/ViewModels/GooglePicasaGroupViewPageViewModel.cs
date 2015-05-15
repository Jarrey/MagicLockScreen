using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_GooglePicasaService.Models;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Contract;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MagicLockScreen_Service_GooglePicasaService.UI.ViewModels
{
    public class GooglePicasaGroupViewPageViewModel : ViewModelBase
    {
        private readonly GooglePicasaBackgroundTaskService googlePicasaBackgroundTaskService;
        private readonly GooglePicasaQueryService googlePicasaQueryService;
        private readonly GooglePicasaServiceChannel googlePicasaServiceChannel;

        public GooglePicasaGroupViewPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            googlePicasaServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["GPSC"] as GooglePicasaServiceChannel;
            googlePicasaQueryService = googlePicasaServiceChannel["GPQS"] as GooglePicasaQueryService;
            if (googlePicasaQueryService.IsSupportBackgroundTask)
                googlePicasaBackgroundTaskService =
                    googlePicasaQueryService.BackgroundTaskService as GooglePicasaBackgroundTaskService;

            this["GooglePicasaSelectedGroup"] = null;
            this["GooglePicasaSelectedItem"] = null;

            if (googlePicasaBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = googlePicasaBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
            }

            #region Commands

            #region RegisterBackgroundTaskCommand

            this["RegisterBackgroundTaskCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        if (googlePicasaBackgroundTaskService != null)
                            googlePicasaBackgroundTaskService.InitializeBackgroundTask(
                                new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                null);

                        dynamic selectTriggerTimeDesc =
                            (from dynamic obj in googlePicasaBackgroundTaskService.TimeTriggerTimes
                             where obj.Value == this["BackgroundTaskTimeTiggerTime"].ToString()
                             select obj.Name).First();
                        new MessagePopup(string.Format(ResourcesLoader.Loader["SetBackgroundTaskSucessfully"],
                                                       selectTriggerTimeDesc)).Show();
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });

            #endregion

            #region SetAsLockScreenCommand

            this["SetAsLockScreenCommand"] = ApplicationHelper.SetAsLockScreenCommand;

            #endregion

            #region SetAppBackgroundCommand

            this["SetAppBackgroundCommand"] = ApplicationHelper.SetAppBackgroundCommand;

            #endregion

            #region RefreshCommand

            this["RefreshCommand"] = new RelayCommand(() =>
                {
                    (this["GooglePicasaSelectedGroup"] as GooglePicasaGroup).GooglePicasas.RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] =
                (googlePicasaServiceChannel.Model as GooglePicasaServiceChannelModel).ShowInfoCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["GooglePicasaSelectedItem"] as GooglePicasa).Title,
                                                        (this["GooglePicasaSelectedItem"] as GooglePicasa).Summary);
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });

            #endregion

            #endregion
        }

        public override void LoadState()
        {
        }

        public override void SaveState(Dictionary<string, object> pageState)
        {
        }
    }
}