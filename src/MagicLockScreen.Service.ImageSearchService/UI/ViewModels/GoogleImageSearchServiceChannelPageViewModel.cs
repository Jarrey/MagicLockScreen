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
    public class GoogleImageSearchServiceChannelPageViewModel : ViewModelBase, ISearchable
    {
        private readonly GoogleImageSearchBackgroundTaskService googleImageSearchBackgroundTaskService;
        private readonly GoogleImageSearchService googleImageSearchService;
        private readonly GoogleImageSearchServiceChannel googleImageSearchServiceChannel;

        public GoogleImageSearchServiceChannelPageViewModel(FrameworkElement view, Dictionary<string, object> pageState)
            :
                base(view, pageState)
        {
            googleImageSearchServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["GISSC"] as GoogleImageSearchServiceChannel;
            googleImageSearchService = googleImageSearchServiceChannel["GISS"] as GoogleImageSearchService;
            if (googleImageSearchService.IsSupportBackgroundTask)
                googleImageSearchBackgroundTaskService =
                    googleImageSearchService.BackgroundTaskService as GoogleImageSearchBackgroundTaskService;

            this["PageTitle"] = ResourcesLoader.Loader["GoogleSearchProviderTitle"];
            this["PromptOpacity"] = 0.0;
            this["ImageCollection"] = null;
            this["ImageSelectedItem"] = null;

            if (googleImageSearchBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = googleImageSearchBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = googleImageSearchBackgroundTaskService;
                this["UpdateLockScreen"] = true;
                this["UpdateWallpaper"] = true;
            }

            this["IsBackgroundTaskPopupOpen"] = false;
            this["ShowSaveButton"] = false;
            this["ShowRemoveButton"] = false;
            this["EnableAppBar"] = false;

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
                            string parameters = string.Format("LockScreen:{0}|Wallpaper:{1}|keyword:{2}", (bool)this["UpdateLockScreen"], (bool)this["UpdateWallpaper"], _queryText);

                            if (googleImageSearchBackgroundTaskService != null)
                                googleImageSearchBackgroundTaskService.InitializeBackgroundTask(
                                    new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                    null,
                                    parameters);

                            dynamic selectTriggerTimeDesc =
                                (from dynamic obj in googleImageSearchBackgroundTaskService.TimeTriggerTimes
                                 where obj.Value == this["BackgroundTaskTimeTiggerTime"].ToString()
                                 select obj.Name).First();
                            new MessagePopup(
                                string.Format(
                                    MagicLockScreen_Helper.Resources.ResourcesLoader.Loader["SetBackgroundTaskSucessfully"],
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
                    if (googleImageSearchBackgroundTaskService != null)
                    {
                        googleImageSearchBackgroundTaskService.UnregisterBackgroundTask();
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

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region RefreshCommand

            this["RefreshCommand"] = new RelayCommand(() =>
                {
                    (this["ImageCollection"] as GoogleImageCollection).RefreshCollection();
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveQueryCommand

            this["SaveQueryCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        googleImageSearchServiceChannel.AddModel(googleImageSearchServiceChannel, _queryText,
                                                                 ResourcesLoader.Loader["GoogleSearchSubTitle"]);
                        googleImageSearchServiceChannel.SaveModels();
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
                        if (await googleImageSearchServiceChannel.RemoveModel(_queryText))
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
                        XamlReader.Load(string.Format(ResourcesLoader.Loader["GoogleServiceChannelInformation"],
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

        /// <summary>
        ///     Method implemented from ISearchable interface
        /// </summary>
        /// <param name="queryText"></param>
        public void QueryData(string queryText)
        {
            _queryText = queryText.Trim();
            this["PromptOpacity"] = 1.0;
            this["ImageCollection"] = new GoogleImageCollection(googleImageSearchService.MaxItemCount,
                                                                new IService[] {googleImageSearchService}, queryText);
            this["PageTitle"] = _queryText + " - " + ResourcesLoader.Loader["GoogleSearchSubTitle"];

            if (googleImageSearchServiceChannel.ContainsModel(_queryText))
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