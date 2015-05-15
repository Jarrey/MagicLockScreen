using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_ImageSearchService.Models;
using MagicLockScreen_Service_ImageSearchService.Resources;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Contract;
using NoteOne_Core.Interfaces;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using System.Globalization;

namespace MagicLockScreen_Service_ImageSearchService.UI.ViewModels
{
    public class FlickrImageSearchServiceChannelPageViewModel : ViewModelBase, ISearchable
    {
        private readonly FlickrImageSearchBackgroundTaskService flickrImageSearchBackgroundTaskService;
        private readonly FlickrImageSearchService flickrImageSearchService;
        private readonly FlickrImageSearchServiceChannel flickrImageSearchServiceChannel;

        public FlickrImageSearchServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState)
            :
                base(view, pageState)
        {
            flickrImageSearchServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["FISSC"] as FlickrImageSearchServiceChannel;
            flickrImageSearchService = flickrImageSearchServiceChannel["FISS"] as FlickrImageSearchService;
            if (flickrImageSearchService.IsSupportBackgroundTask)
                flickrImageSearchBackgroundTaskService =
                    flickrImageSearchService.BackgroundTaskService as FlickrImageSearchBackgroundTaskService;

            this["PageTitle"] = ResourcesLoader.Loader["FlickrSearchProviderTitle"];
            this["PromptOpacity"] = 0.0;
            this["ImageCollection"] = null;
            this["ImageSelectedItem"] = null;

            if (flickrImageSearchBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = flickrImageSearchBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = flickrImageSearchBackgroundTaskService;
            }

            this["ShowSaveButton"] = false;
            this["ShowRemoveButton"] = false;
            this["EnableAppBar"] = false;

            #region Commands

            #region RegisterBackgroundTaskCommand

            this["RegisterBackgroundTaskCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        if (flickrImageSearchBackgroundTaskService != null)
                            flickrImageSearchBackgroundTaskService.InitializeBackgroundTask(
                                new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                null,
                                "keyword:" + _queryText);

                        dynamic selectTriggerTimeDesc =
                            (from dynamic obj in flickrImageSearchBackgroundTaskService.TimeTriggerTimes
                             where obj.Value == this["BackgroundTaskTimeTiggerTime"].ToString()
                             select obj.Name).First();
                        new MessagePopup(
                            string.Format(
                                MagicLockScreen_Helper.Resources.ResourcesLoader.Loader["SetBackgroundTaskSucessfully"],
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
                    if (flickrImageSearchBackgroundTaskService != null)
                    {
                        flickrImageSearchBackgroundTaskService.UnregisterBackgroundTask();
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

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region RefreshCommand

            this["RefreshCommand"] = new RelayCommand(() =>
                {
                    (this["ImageCollection"] as FlickrImageCollection).RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveQueryCommand

            this["SaveQueryCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        flickrImageSearchServiceChannel.AddModel(flickrImageSearchServiceChannel, _queryText,
                                                                 ResourcesLoader.Loader["FlickrSearchSubTitle"]);
                        flickrImageSearchServiceChannel.SaveModels();
                        ShowRemoveButton();
                    }
                    catch (Exception ex)
                    {
                        ShowSaveButton();
                        ex.WriteLog();
                    }
                });

            #endregion

            #region RemoveQueryCommand

            this["RemoveQueryCommand"] = new RelayCommand(async () =>
                {
                    try
                    {
                        if (await flickrImageSearchServiceChannel.RemoveModel(_queryText))
                            ShowSaveButton();
                    }
                    catch (Exception ex)
                    {
                        ShowRemoveButton();
                        ex.WriteLog();
                    }
                });

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] = new RelayCommand(() =>
                {
                    var information =
                        XamlReader.Load(string.Format(ResourcesLoader.Loader["FlickrServiceChannelInformation"],
                                                      _queryText)) as UIElement;
                    if (information != null)
                    {
                        var settingPopup = new SettingPopup(ResourcesLoader.Loader["InformationTitle"])
                            {
                                Content = information
                            };
                        settingPopup.Show();
                    }
                });

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var shareImage = new ShareImage(imageFile,
                                                        (this["ImageSelectedItem"] as SearchImage).Title,
                                                        (this["ImageSelectedItem"] as SearchImage).Content);
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

        /// <summary>
        ///     Method implemented from ISearchable interface
        /// </summary>
        /// <param name="queryText"></param>
        public void QueryData(string queryText)
        {
            _queryText = queryText.Trim();
            this["PromptOpacity"] = 1.0;
            this["ImageCollection"] = new FlickrImageCollection(flickrImageSearchService.MaxItemCount,
                                                                new IService[] {flickrImageSearchService}, queryText);
            this["PageTitle"] = _queryText + " - " + ResourcesLoader.Loader["FlickrSearchSubTitle"];

            if (flickrImageSearchServiceChannel.ContainsModel(_queryText))
                ShowRemoveButton();
            else
                ShowSaveButton();

            this["EnableAppBar"] = true;
        }

        #region Fields

        private string _queryText = string.Empty;

        #endregion

        public override void LoadState()
        {
        }

        public override void SaveState(Dictionary<string, object> pageState)
        {
        }

        private void ShowSaveButton()
        {
            this["ShowSaveButton"] = true;
            this["ShowRemoveButton"] = false;
        }

        private void ShowRemoveButton()
        {
            this["ShowSaveButton"] = false;
            this["ShowRemoveButton"] = true;
        }
    }
}