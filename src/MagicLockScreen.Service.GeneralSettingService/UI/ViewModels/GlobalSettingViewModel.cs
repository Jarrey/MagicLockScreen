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