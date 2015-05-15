using System;
using System.Collections.Generic;
using System.Linq;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Models;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.Interfaces;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;
using Windows.UI.Xaml;
using AppSettings = NoteOne_Utility.AppSettings;
using Windows.Storage;
using Windows.UI.Popups;
using MagicLockScreen_Service_GeneralSettingService.Resources;
using Windows.Storage.Search;

namespace MagicLockScreen_Service_GeneralSettingService.UI.ViewModels
{
    public class GlobalSettingViewModel : ViewModelBase
    {
        public GlobalSettingViewModel(FrameworkElement view, Dictionary<string, object> pageState)
            : base(view, pageState)
        {
            this["GLOBAL"] = AppSettings.Instance.Settings;
            this["APP"] = MagicLockScreen_Helper.AppSettings.Instance.Settings;
            this["LOCAL_APP"] = LocalAppSettings.Instance.Settings;

            // read image search providers
            IList<string> providers = new List<string>();
            foreach (var provider in Enum.GetValues(typeof(SearchProvider)))
            {
                providers.Add(provider.GetDescription());
            }
            this["SearchProviders"] = providers;

            this["OnlineChannels"] =
                from ServiceChannelModel model in ServiceChannelManager.CurrentServiceChannelManager.AvailableModels
                where model.GroupID == ServiceChannelGroupID.OnlinePictures
                select model;

            this["SaveImagePositions"] = SaveImagePosition.SaveImagePositions;

            UpdateCustomizeChannelModels();
            UpdateLocalPictureChannelModels();

            #region Commands

            #region LoadedCommand

            this["LoadedCommand"] = new RelayCommand(() =>
                {
                    if (SettingPopup.Current != null)
                    {
                        SettingPopup.Current.PopupClosed += async (o, e) =>
                            {
                                await AppSettings.SaveSettings(AppSettings.Instance);
                                await AppSettings.SaveSettings(MagicLockScreen_Helper.AppSettings.Instance);
                                await AppSettings.SaveSettings(LocalAppSettings.Instance);
                            };
                    }
                });

            #endregion

            #region RemoveCustomizePictureModelCommand

            this["RemoveCustomizePictureModelCommand"] = new RelayCommand<ServiceChannelModel>(async p =>
                {
                    var serviceChannel = p.Channel as ISearchServiceChannel;
                    if (serviceChannel != null)
                    {
                        await serviceChannel.RemoveModel(p.Title);
                        UpdateCustomizeChannelModels();
                        SettingPopup.Current.Show();
                    }
                });

            #endregion

            #region RemoveCustomizePictureModelCommand

            this["RemoveLocalPictureModelCommand"] = new RelayCommand<ServiceChannelModel>(async p =>
                {
                    var serviceChannelModel = p as MagicLockScreenServiceChannelModel;
                    if (serviceChannelModel != null)
                    {
                        var serviceChannel = serviceChannelModel.Channel as ISearchServiceChannel;
                        if (serviceChannel != null)
                        {
                            await serviceChannel.RemoveModel(serviceChannelModel.Path);
                            UpdateLocalPictureChannelModels();
                            SettingPopup.Current.Show();
                        }
                    }
                });

            #endregion

            #region Clean all local settings command

            #endregion

            this["CleanLocalSettingsCommand"] = new RelayCommand(async () =>
                {
                    var dialog = new MessageDialog(ResourcesLoader.Loader["CleanSettingsQuestion"]);
                    dialog.Commands.Add(new UICommand(ResourcesLoader.Loader["OKButton"], null, 1));
                    dialog.Commands.Add(new UICommand(ResourcesLoader.Loader["CancelButton"], null, 0));
                    IUICommand command = await dialog.ShowAsync();

                    if ((int)command.Id == 1)  // User click OK
                    {
                        foreach (StorageFile file in await ApplicationData.Current.LocalFolder.GetFilesAsync())
                            await file.DeleteAsync();
                        foreach (StorageFile file in await ApplicationData.Current.RoamingFolder.GetStorageFiles(new string[] { ".setting" }, FolderDepth.Shallow))
                            await file.DeleteAsync();

                        LocalAppSettings.Instance.Reset();
                        AppSettings.Instance.Reset();
                        MagicLockScreen_Helper.AppSettings.Instance.Reset();

                        // unregister all UpdateLockScreenBackgroundTask tasks from previous version
                        BackgroundTaskController.UnregisterBackgroundTasks("UpdateLockScreenBackgroundTask");
                    }

                    SettingPopup.Current.Show();
                });

            #endregion
        }

        #region Helper Methods

        private void UpdateLocalPictureChannelModels()
        {
            ServiceChannelGroupModel localPictureGroupModel =
                ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[
                    ServiceChannelGroupID.LocalPictures];
            if (localPictureGroupModel != null && localPictureGroupModel.Models != null)
            {
                this["LocalPictureChannels"] = from ServiceChannelModel model in localPictureGroupModel.Models
                                               where model.Title != ConstKeys.ADD_SERVICE_CHANNEL_MODEL_KEY
                                               select model;
            }
        }

        private void UpdateCustomizeChannelModels()
        {
            ServiceChannelGroupModel customPictureGroupModel =
                ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[
                    ServiceChannelGroupID.CustomSearchPictures];
            if (customPictureGroupModel != null && customPictureGroupModel.Models != null)
            {
                this["CustomizeChannels"] = from ServiceChannelModel model in customPictureGroupModel.Models
                                            where model.Title != ConstKeys.ADD_SERVICE_CHANNEL_MODEL_KEY
                                            select model;
            }
        }

        #endregion

        public override void LoadState()
        {
        }

        public override void SaveState(Dictionary<string, object> pageState)
        {
        }
    }
}