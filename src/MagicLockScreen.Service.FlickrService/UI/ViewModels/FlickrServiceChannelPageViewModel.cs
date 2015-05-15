using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_FlickrService.Models;
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

namespace MagicLockScreen_Service_FlickrService.UI.ViewModels
{
    public class FlickrServiceChannelPageViewModel : ViewModelBase
    {
        private readonly FlickrBackgroundTaskService flickrBackgroundTaskService;
        private readonly FlickrQueryService flickrQueryService;
        private readonly FlickrServiceChannel flickrServiceChannel;

        public FlickrServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            flickrServiceChannel = ServiceChannelManager.CurrentServiceChannelManager["FSC"] as FlickrServiceChannel;
            flickrQueryService = flickrServiceChannel["FQS"] as FlickrQueryService;
            if (flickrQueryService.IsSupportBackgroundTask)
                flickrBackgroundTaskService = flickrQueryService.BackgroundTaskService as FlickrBackgroundTaskService;

            this["FlickrCollection"] = new FlickrCollection(flickrQueryService.MaxItemCount,
                                                            new IService[] {flickrQueryService});
            this["FlickrSelectedItem"] = null;

            if (flickrBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = flickrBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = flickrBackgroundTaskService;
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

                            if (flickrBackgroundTaskService != null)
                                flickrBackgroundTaskService.InitializeBackgroundTask(
                                    new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                    null, parameters);

                            dynamic selectTriggerTimeDesc =
                                (from dynamic obj in flickrBackgroundTaskService.TimeTriggerTimes
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
                    if (flickrBackgroundTaskService != null)
                    {
                        flickrBackgroundTaskService.UnregisterBackgroundTask();
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
                    (this["FlickrCollection"] as FlickrCollection).RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] = (flickrServiceChannel.Model as FlickrServiceChannelModel).ShowInfoCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["FlickrSelectedItem"] as Flickr).Title,
                                                        (this["FlickrSelectedItem"] as Flickr).Description);
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