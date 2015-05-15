using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_LunarPODService.Models;
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
using Windows.UI.Xaml.Controls;
using System.Globalization;

namespace MagicLockScreen_Service_LunarPODService.UI.ViewModels
{
    public class LunarPODServiceChannelPageViewModel : ViewModelBase
    {
        private readonly LunarPODBackgroundTaskService lunarPODBackgroundTaskService;
        private readonly LunarPODQueryService lunarPODQueryService;
        private readonly LunarPODServiceChannel lunarPODServiceChannel;

        public LunarPODServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            lunarPODServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["LPSC"] as LunarPODServiceChannel;
            lunarPODQueryService = lunarPODServiceChannel["LPQS"] as LunarPODQueryService;
            if (lunarPODQueryService.IsSupportBackgroundTask)
                lunarPODBackgroundTaskService =
                    lunarPODQueryService.BackgroundTaskService as LunarPODBackgroundTaskService;


            this["LunarPODCollection"] = new LunarPODCollection(lunarPODQueryService.MaxItemCount,
                                                                new IService[] {lunarPODQueryService});
            this["LunarPODSelectedItem"] = null;

            if (lunarPODBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = lunarPODBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = lunarPODBackgroundTaskService;
            }

            #region Commands

            #region RegisterBackgroundTaskCommand

            this["RegisterBackgroundTaskCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        if (lunarPODBackgroundTaskService != null)
                            lunarPODQueryService.BackgroundTaskService.InitializeBackgroundTask(
                                new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                null);

                        dynamic selectTriggerTimeDesc =
                            (from dynamic obj in lunarPODBackgroundTaskService.TimeTriggerTimes
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

            #region UnregisterBackgroundTaskCommand

            this["UnregisterBackgroundTaskCommand"] = new RelayCommand(() =>
            {
                try
                {
                    if (lunarPODBackgroundTaskService != null)
                    {
                        lunarPODBackgroundTaskService.UnregisterBackgroundTask();
                        new MessagePopup(ResourcesLoader.Loader["UnregisterBackgroundTaskSucessfully"]).Show();
                    }
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
                    (this["LunarPODCollection"] as LunarPODCollection).RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] = (lunarPODServiceChannel.Model as LunarPODServiceChannelModel).ShowInfoCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["LunarPODSelectedItem"] as LunarPOD).Title,
                                                        (this["LunarPODSelectedItem"] as LunarPOD).Explanation);
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });

            #endregion

            #region ZoomInCommand

            this["ZoomInCommand"] = new RelayCommand<object>(p =>
                {
                    var imageScrollViewer = p as ScrollViewer;
                    if (imageScrollViewer != null)
                    {
                        float zoomInterval =
                            AppSettings.Instance[AppSettings.ZOOM_FACTORY_INTERVAL].ToString(CultureInfo.InvariantCulture).StringToFloat();
                        imageScrollViewer.ZoomToFactor(imageScrollViewer.ZoomFactor + zoomInterval);
                    }
                });

            #endregion

            #region ZoomOutCommand

            this["ZoomOutCommand"] = new RelayCommand<object>(p =>
                {
                    var imageScrollViewer = p as ScrollViewer;
                    if (imageScrollViewer != null)
                    {
                        float zoomInterval =
                            AppSettings.Instance[AppSettings.ZOOM_FACTORY_INTERVAL].ToString(CultureInfo.InvariantCulture).StringToFloat();
                        imageScrollViewer.ZoomToFactor(imageScrollViewer.ZoomFactor - zoomInterval);
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