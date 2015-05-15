using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_MicrosoftWallpaperService.Models;
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

namespace MagicLockScreen_Service_MicrosoftWallpaperService.UI.ViewModels
{
    public class MicrosoftWallpaperServiceChannelPageViewModel : ViewModelBase
    {
        private readonly MicrosoftWallpaperBackgroundTaskService microsoftWallpaperBackgroundTaskService;
        private readonly MicrosoftWallpaperQueryService microsoftWallpaperQueryService;
        private readonly MicrosoftWallpaperServiceChannel microsoftWallpaperServiceChannel;

        public MicrosoftWallpaperServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            microsoftWallpaperServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["MSWSC"] as MicrosoftWallpaperServiceChannel;
            microsoftWallpaperQueryService = microsoftWallpaperServiceChannel["MSWQS"] as MicrosoftWallpaperQueryService;
            if (microsoftWallpaperQueryService.IsSupportBackgroundTask)
                microsoftWallpaperBackgroundTaskService =
                    microsoftWallpaperQueryService.BackgroundTaskService as MicrosoftWallpaperBackgroundTaskService;


            this["MicrosoftWallpaperCollection"] = new MicrosoftWallpaperCollection(microsoftWallpaperQueryService.MaxItemCount,
                                                                new IService[] {microsoftWallpaperQueryService});
            this["MicrosoftWallpaperSelectedItem"] = null;

            if (microsoftWallpaperBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = microsoftWallpaperBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = microsoftWallpaperBackgroundTaskService;
                this["UpdateLockScreen"] = true;
                this["UpdateWallpaper"] = true;
            }

            this["IsBackgroundTaskPopupOpen"] = false;

            #region Commands

            #region PopupBackgroundTaskCommand

            this["PopupBackgroundTaskCommand"] = new RelayCommand(() =>
            {
                this["IsBackgroundTaskPopupOpen"] = true;
            });

            #endregion

            #region RegisterBackgroundTaskCommand

            this["RegisterBackgroundTaskCommand"] = new RelayCommand(async () =>
                {
                    try
                    {
                        if ((bool)this["UpdateWallpaper"])
                        {
                            this["UpdateWallpaper"] = await ApplicationHelper.CheckPromptDesktopService();
                        }

                        if ((bool)this["UpdateLockScreen"] || (bool)this["UpdateWallpaper"])
                        {
                            string parameters = string.Format("LockScreen:{0}|Wallpaper:{1}", (bool)this["UpdateLockScreen"], (bool)this["UpdateWallpaper"]);

                            if (microsoftWallpaperBackgroundTaskService != null)
                                microsoftWallpaperQueryService.BackgroundTaskService.InitializeBackgroundTask(
                                    new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                    null, parameters);

                            dynamic selectTriggerTimeDesc =
                                (from dynamic obj in microsoftWallpaperBackgroundTaskService.TimeTriggerTimes
                                 where obj.Value == this["BackgroundTaskTimeTiggerTime"].ToString()
                                 select obj.Name).First();
                            new MessagePopup(string.Format(ResourcesLoader.Loader["SetBackgroundTaskSucessfully"],
                                                           selectTriggerTimeDesc)).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                    finally
                    {
                        this["IsBackgroundTaskPopupOpen"] = false;
                    }
                });

            #endregion

            #region UnregisterBackgroundTaskCommand

            this["UnregisterBackgroundTaskCommand"] = new RelayCommand(() =>
            {
                try
                {
                    if (microsoftWallpaperBackgroundTaskService != null)
                    {
                        microsoftWallpaperBackgroundTaskService.UnregisterBackgroundTask();
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

            #region SetWallpaperCommand

            this["SetWallpaperCommand"] = ApplicationHelper.SetWallpaperCommand;

            #endregion

            #region SetAppBackgroundCommand

            this["SetAppBackgroundCommand"] = ApplicationHelper.SetAppBackgroundCommand;

            #endregion

            #region RefreshCommand

            this["RefreshCommand"] = new RelayCommand(() =>
                {
                    (this["MicrosoftWallpaperCollection"] as MicrosoftWallpaperCollection).RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] = (microsoftWallpaperServiceChannel.Model as MicrosoftWallpaperServiceChannelModel).ShowInfoCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["MicrosoftWallpaperSelectedItem"] as MicrosoftWallpaper).Title,
                                                        (this["MicrosoftWallpaperSelectedItem"] as MicrosoftWallpaper).Explanation);
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
                        imageScrollViewer.ChangeView(null, null, imageScrollViewer.ZoomFactor + zoomInterval);
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
                        imageScrollViewer.ChangeView(null, null, imageScrollViewer.ZoomFactor - zoomInterval);
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