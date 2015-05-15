using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_AstronomyPictureService.Models;
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

namespace MagicLockScreen_Service_AstronomyPictureService.UI.ViewModels
{
    public class AstronomyPictureServiceChannelPageViewModel : ViewModelBase
    {
        private readonly AstronomyPictureBackgroundTaskService astronomyPictureBackgroundTaskService;
        private readonly AstronomyPictureQueryService astronomyPictureQueryService;
        private readonly AstronomyPictureServiceChannel astronomyPictureServiceChannel;

        public AstronomyPictureServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState)
            :
                base(view, pageState)
        {
            astronomyPictureServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["APSC"] as AstronomyPictureServiceChannel;
            astronomyPictureQueryService = astronomyPictureServiceChannel["APQS"] as AstronomyPictureQueryService;
            if (astronomyPictureQueryService.IsSupportBackgroundTask)
                astronomyPictureBackgroundTaskService =
                    astronomyPictureQueryService.BackgroundTaskService as AstronomyPictureBackgroundTaskService;


            this["AstronomyPictureCollection"] =
                new AstronomyPictureCollection(astronomyPictureQueryService.MaxItemCount,
                                               new IService[] { astronomyPictureQueryService });
            this["AstronomyPictureSelectedItem"] = null;

            if (astronomyPictureBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = astronomyPictureBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = astronomyPictureBackgroundTaskService;
            }

            #region Commands

            #region RegisterBackgroundTaskCommand

            this["RegisterBackgroundTaskCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        if (astronomyPictureBackgroundTaskService != null)
                            astronomyPictureBackgroundTaskService.InitializeBackgroundTask(
                                new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                null);

                        dynamic selectTriggerTimeDesc =
                            (from dynamic obj in astronomyPictureBackgroundTaskService.TimeTriggerTimes
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
                    if (astronomyPictureBackgroundTaskService != null)
                    {
                        astronomyPictureBackgroundTaskService.UnregisterBackgroundTask();
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
                    (this["AstronomyPictureCollection"] as AstronomyPictureCollection).RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] =
                (astronomyPictureServiceChannel.Model as AstronomyPictureServiceChannelModel).ShowInfoCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["AstronomyPictureSelectedItem"] as AstronomyPicture).Title,
                                                        (this["AstronomyPictureSelectedItem"] as AstronomyPicture)
                                                            .Explanation);
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