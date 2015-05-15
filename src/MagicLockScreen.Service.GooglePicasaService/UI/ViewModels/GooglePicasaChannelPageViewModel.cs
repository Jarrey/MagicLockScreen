using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_GooglePicasaService.Models;
using MagicLockScreen_Service_GooglePicasaService.UI.Views;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Globalization;

namespace MagicLockScreen_Service_GooglePicasaService.UI.ViewModels
{
    public class GooglePicasaServiceChannelPageViewModel : ViewModelBase
    {
        private readonly GooglePicasaBackgroundTaskService googlePicasaBackgroundTaskService;
        private readonly GooglePicasaQueryService googlePicasaQueryService;
        private readonly GooglePicasaServiceChannel googlePicasaServiceChannel;

        public GooglePicasaServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            googlePicasaServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["GPSC"] as GooglePicasaServiceChannel;
            googlePicasaQueryService = googlePicasaServiceChannel["GPQS"] as GooglePicasaQueryService;
            if (googlePicasaQueryService.IsSupportBackgroundTask)
                googlePicasaBackgroundTaskService =
                    googlePicasaQueryService.BackgroundTaskService as GooglePicasaBackgroundTaskService;

            this["GooglePicasaGroups"] = new GooglePicasaGroupCollection(
                new IService[] {googlePicasaQueryService},
                googlePicasaQueryService.ConfigXml,
                googlePicasaQueryService.MaxItemCount);

            if (googlePicasaBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = googlePicasaBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = googlePicasaBackgroundTaskService;
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

            #region UnregisterBackgroundTaskCommand

            this["UnregisterBackgroundTaskCommand"] = new RelayCommand(() =>
            {
                try
                {
                    if (googlePicasaBackgroundTaskService != null)
                    {
                        googlePicasaBackgroundTaskService.UnregisterBackgroundTask();
                        new MessagePopup(ResourcesLoader.Loader["UnregisterBackgroundTaskSucessfully"]).Show();
                    }
                }
                catch (Exception ex)
                {
                    ex.WriteLog();
                }
            });

            #endregion

            #region NavigateGooglePicasaGroupViewPageCommand

            this["NavigateGooglePicasaGroupViewPageCommand"] = new RelayCommand<object>(p =>
                {
                    var navigationFrame = Window.Current.Content as Frame;
                    if (navigationFrame != null)
                    {
                        string key = "NavigateGooglePicasaGroupViewPageKey";
                        if (CoreApplication.Properties.ContainsKey(key))
                            CoreApplication.Properties.Remove(key);
                        CoreApplication.Properties.Add(key, p);
                        navigationFrame.Navigate(typeof (GooglePicasaGroupViewPage));
                    }
                });

            #endregion

            #region RefreshCommand

            this["RefreshCommand"] =
                new RelayCommand(
                    () => { (this["GooglePicasaGroups"] as GooglePicasaGroupCollection).RefreshCollection(); });

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] =
                (googlePicasaServiceChannel.Model as GooglePicasaServiceChannelModel).ShowInfoCommand;

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