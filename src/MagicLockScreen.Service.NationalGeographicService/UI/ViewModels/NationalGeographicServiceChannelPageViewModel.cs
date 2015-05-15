using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_NationalGeographicService.Models;
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

namespace MagicLockScreen_Service_NationalGeographicService.UI.ViewModels
{
    public class NationalGeographicServiceChannelPageViewModel : ViewModelBase
    {
        private readonly NationalGeographicBackgroundTaskService nationalGeographicBackgroundTaskService;
        private readonly NationalGeographicQueryService nationalGeographicQueryService;
        private readonly NationalGeographicServiceChannel nationalGeographicServiceChannel;

        public NationalGeographicServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState)
            :
                base(view, pageState)
        {
            nationalGeographicServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["NGSC"] as NationalGeographicServiceChannel;
            nationalGeographicQueryService = nationalGeographicServiceChannel["NGQS"] as NationalGeographicQueryService;
            if (nationalGeographicQueryService.IsSupportBackgroundTask)
                nationalGeographicBackgroundTaskService =
                    nationalGeographicQueryService.BackgroundTaskService as NationalGeographicBackgroundTaskService;

            this["NationalGeographicCollection"] =
                new NationalGeographicCollection(nationalGeographicQueryService.MaxItemCount,
                                                 new IService[] {nationalGeographicQueryService});
            this["NationalGeographicSelectedItem"] = null;

            if (nationalGeographicQueryService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = nationalGeographicBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "720";
                this["BackgroundTaskService"] = nationalGeographicBackgroundTaskService;
            }

            #region Commands

            #region RegisterBackgroundTaskCommand

            this["RegisterBackgroundTaskCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        if (nationalGeographicBackgroundTaskService != null)
                            nationalGeographicBackgroundTaskService.InitializeBackgroundTask(
                                new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                null);

                        dynamic selectTriggerTimeDesc =
                            (from dynamic obj in nationalGeographicBackgroundTaskService.TimeTriggerTimes
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
                    if (nationalGeographicBackgroundTaskService != null)
                    {
                        nationalGeographicBackgroundTaskService.UnregisterBackgroundTask();
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
                    (this["NationalGeographicCollection"] as NationalGeographicCollection).RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] =
                (nationalGeographicServiceChannel.Model as NationalGeographicServiceChannelModel).ShowInfoCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["NationalGeographicSelectedItem"] as NationalGeographic)
                                                            .Title,
                                                        (this["NationalGeographicSelectedItem"] as NationalGeographic)
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