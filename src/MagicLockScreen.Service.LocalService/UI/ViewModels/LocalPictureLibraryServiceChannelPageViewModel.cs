using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_LocalService.Models;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Contract;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Globalization;

namespace MagicLockScreen_Service_LocalService.UI.ViewModels
{
    public class LocalPictureLibraryServiceChannelPageViewModel : ViewModelBase
    {
        private readonly LocalPictureLibraryBackgroundTaskService localPictureLibraryBackgroundTaskService;
        private readonly LocalPictureLibraryQueryService localPictureLibraryQueryService;
        private readonly LocalPictureLibraryServiceChannel localPictureLibraryServiceChannel;

        public LocalPictureLibraryServiceChannelPageViewModel(FrameworkElement view,
                                                              Dictionary<string, object> pageState,
                                                              LocalPictureLibraryServiceChannelModel model) :
                                                                  base(view, pageState)
        {
            localPictureLibraryServiceChannel =
                ServiceChannelManager.CurrentServiceChannelManager["LPLSC"] as LocalPictureLibraryServiceChannel;
            localPictureLibraryQueryService =
                localPictureLibraryServiceChannel["LPLQS"] as LocalPictureLibraryQueryService;
            if (localPictureLibraryQueryService.IsSupportBackgroundTask)
                localPictureLibraryBackgroundTaskService =
                    localPictureLibraryQueryService.BackgroundTaskService as LocalPictureLibraryBackgroundTaskService;

            string key = ConstKeys.LOCAL_PICTURE_LIBRARY_VIEW_PAGE_KEY;
            if (CoreApplication.Properties.ContainsKey(key))
                CoreApplication.Properties.Remove(key);
            CoreApplication.Properties.Add(key, model);

            this["CurrentModel"] = model;
            this["CurrentPath"] = model.Path;
            this["Title"] = model.Title;

            if (localPictureLibraryServiceChannel.ContainsModel(model.Path))
                ShowRemoveButton();
            else
                ShowSaveButton();

            localPictureLibraryQueryService.GetFilesAsync(model.Path).ContinueWith(p =>
                {
                    if (p.Status == TaskStatus.RanToCompletion && p.Result != null)
                        this["LocalPictureLibraryCollection"] = new LocalPictureLibraryCollection(
                            (uint) p.Result.Count, new IService[] {localPictureLibraryQueryService}, model.Path);
                    else
                        this["LocalPictureLibraryCollection"] = null;
                });
            this["LocalPictureLibrarySelectedItem"] = null;

            if (localPictureLibraryBackgroundTaskService != null)
            {
                this["BackgroundTaskTimeTiggerTimes"] = localPictureLibraryBackgroundTaskService.TimeTriggerTimes;
                this["BackgroundTaskTimeTiggerTime"] = "15";
                this["BackgroundTaskService"] = localPictureLibraryBackgroundTaskService;
            }

            #region Commands

            #region RegisterBackgroundTaskCommand

            this["RegisterBackgroundTaskCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        if (localPictureLibraryBackgroundTaskService != null)
                            localPictureLibraryBackgroundTaskService.InitializeBackgroundTask(
                                new TimeTrigger(this["BackgroundTaskTimeTiggerTime"].ToString().StringToUInt(), false),
                                null,
                                ConstKeys.LOCAL_FOLDER_PATH_KEY + ":" + this["CurrentPath"].ToString().Replace(':', '?'));

                        dynamic selectTriggerTimeDesc =
                            (from dynamic obj in localPictureLibraryBackgroundTaskService.TimeTriggerTimes
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
                    if (localPictureLibraryBackgroundTaskService != null)
                    {
                        localPictureLibraryBackgroundTaskService.UnregisterBackgroundTask();
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

            this["RefreshCommand"] = new RelayCommand(async () =>
                {
                    var maxCount =
                        (uint)
                        (await
                         localPictureLibraryQueryService.GetFilesAsync(
                             (this["CurrentModel"] as LocalPictureLibraryServiceChannelModel).Path)).Count;
                    (this["LocalPictureLibraryCollection"] as LocalPictureLibraryCollection).RefreshCollection(maxCount);
                    (View as LayoutAwarePage).RefreshContent();
                });

            #endregion

            #region SaveAsCommand

            this["SaveAsCommand"] = ApplicationHelper.SaveAsCommand;

            #endregion

            #region ShowInfoCommand

            this["ShowInfoCommand"] = (this["CurrentModel"] as LocalPictureLibraryServiceChannelModel).ShowInfoCommand;

            #endregion

            #region ShareCommand

            this["ShareCommand"] = new RelayCommand<string>(async url =>
                {
                    try
                    {
                        StorageFile imageFile = await ApplicationHelper.GetTemporaryStorageImageAsync(url);
                        var localPictureLibrary = this["LocalPictureLibrarySelectedItem"] as LocalPictureLibrary;
                        if (localPictureLibrary != null)
                        {
                            var shareImage = new ShareImage(imageFile,
                                                            localPictureLibrary
                                                                .Title,
                                                            localPictureLibrary
                                                                .Explanation);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });

            #endregion

            #region SaveFolderCommand

            this["SaveFolderCommand"] = new RelayCommand(() =>
                {
                    try
                    {
                        localPictureLibraryServiceChannel.AddModel(localPictureLibraryServiceChannel,
                                                                   this["CurrentPath"].ToString());
                        localPictureLibraryServiceChannel.SaveModels();
                        ShowRemoveButton();
                    }
                    catch (Exception ex)
                    {
                        ShowSaveButton();
                        ex.WriteLog();
                    }
                });

            #endregion

            #region RemoveFolderCommand

            this["RemoveFolderCommand"] = new RelayCommand(async () =>
                {
                    try
                    {
                        if (await localPictureLibraryServiceChannel.RemoveModel(this["CurrentPath"].ToString()))
                            ShowSaveButton();
                    }
                    catch (Exception ex)
                    {
                        ShowRemoveButton();
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