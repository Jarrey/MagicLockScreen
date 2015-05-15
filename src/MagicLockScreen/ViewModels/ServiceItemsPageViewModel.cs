using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_GeneralSettingService.UI.Views;
using MagicLockScreen_UI.Resources;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.UI.Common;
using NoteOne_Utility;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Search;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using ConstKeys = NoteOne_Utility.ConstKeys;

namespace MagicLockScreen_UI.ViewModels
{
    public class ServiceItemsPageViewModel : ViewModelBase
    {
        private static bool isAddedSettingPanes;

        public ServiceItemsPageViewModel(FrameworkElement view, Dictionary<string, object> pageState) :
            base(view, pageState)
        {
            this["ServiceChannelGroups"] = ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups;
            this["ServiceChannels"] = ServiceChannelManager.CurrentServiceChannelManager.ServiceChannels;
            this["AppBackground"] = Application.Current.Resources["appBackground"] as ImageBrush;
            this["LOCAL_SETTINGS"] = LocalAppSettings.Instance.Settings;

            #region Commands

            this["PageLoadedCommand"] = new RelayCommand(() => { });

            this["ShowHelpCommand"] = new RelayCommand(() => ShowHelpPopup());

            this["ShowInfoCommand"] = new RelayCommand(() => ShowInformationPopup());

            this["ShowSettingCommand"] = new RelayCommand(() => ShowSettingPopup());

            this["RefreshCommand"] = new RelayCommand(() => { InitializeLogos(); });

            this["SearchCommand"] = new RelayCommand(() =>
                {
                    SearchPane searchPane = SearchPane.GetForCurrentView();
                    searchPane.Show();
                });

            this["ItemClickCommand"] = new RelayCommand<object[]>(p =>
                {
                    if (p != null && p.Length > 1 && p[1] is ItemClickEventArgs)
                    {
                        var serviceChannelModel = (p[1] as ItemClickEventArgs).ClickedItem as ServiceChannelModel;
                        if (serviceChannelModel != null)
                        {
                            var propInfo =
                                serviceChannelModel.GetType()
                                                   .GetTypeInfo()
                                                   .GetMemberInfo("ItemClickCommand", MemberType.Property) as
                                PropertyInfo;
                            if (propInfo != null)
                            {
                                object command = propInfo.GetValue(serviceChannelModel);
                                if (command != null && command is RelayCommand<object[]>)
                                {
                                    (command as RelayCommand<object[]>).Execute(p);
                                }
                            }
                        }
                    }
                });

            #endregion

            #region Initialize Logos

            InitializeLogos();

            #endregion

            #region Register Charm Setting Panel

            if (!isAddedSettingPanes)
            {
                SettingsPane.GetForCurrentView().CommandsRequested += RegisterSettingPanes;
                isAddedSettingPanes = true;
            }

            #endregion
        }

        public override void LoadState()
        {
        }

        public override void SaveState(Dictionary<string, object> pageState)
        {
        }

        /// <summary>
        ///     Initialize logos of all service channels
        /// </summary>
        private void InitializeLogos()
        {
            try
            {
                var serviceChannels = this["ServiceChannels"] as ObservableCollection<IServiceChannel>;
                if (serviceChannels != null)
                    foreach (
                        IServiceChannel serviceChannel in serviceChannels)
                        serviceChannel.InitializeLogo();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private void RegisterSettingPanes(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
        {
            //SettingsCommand accountsCommand = new SettingsCommand("accountsPage", Resources.ResourcesLoader.Loader["Accounts"], command => ShowSettingPopup(SettingSourceFlag.ByCharm));
            //e.Request.ApplicationCommands.Add(accountsCommand);

            var settingCommand = new SettingsCommand("settingPage", ResourcesLoader.Loader["Setting"],
                                                     command => ShowSettingPopup(SettingSourceFlag.ByCharm));
            e.Request.ApplicationCommands.Add(settingCommand);

            // Show help guideline
            var helpCommand = new SettingsCommand("helpPage", ResourcesLoader.Loader["Help"], command => ShowHelpPopup());
            e.Request.ApplicationCommands.Add(helpCommand);

            // For Privacy Policy
            var privacyPolicyCommand = new SettingsCommand("privacyPolicyPage", ResourcesLoader.Loader["PrivacyPolicy"],
                                                           command =>
                                                               {
                                                                   var information =
                                                                       XamlReader.Load(
                                                                           ResourcesLoader.Loader["PrivacyPolicyContent"
                                                                               ]) as UIElement;
                                                                   if (information != null)
                                                                   {
                                                                       var settingPopup =
                                                                           new SettingPopup(
                                                                               ResourcesLoader.Loader["PrivacyPolicy"],
                                                                               SettingSourceFlag.ByCharm)
                                                                               {
                                                                                   Content = information
                                                                               };
                                                                       settingPopup.Show();
                                                                   }
                                                               });
            e.Request.ApplicationCommands.Add(privacyPolicyCommand);

            var aboutCommand = new SettingsCommand("aboutPage", ResourcesLoader.Loader["About"],
                                                   command => ShowInformationPopup(SettingSourceFlag.ByCharm));
            e.Request.ApplicationCommands.Add(aboutCommand);
        }

        private void ShowInformationPopup(SettingSourceFlag flag = SettingSourceFlag.ByApp)
        {
            var information = XamlReader.Load(ResourcesLoader.Loader["PrimaryInformation"]) as UIElement;
            if (information != null)
            {
                var settingPopup = new SettingPopup(ResourcesLoader.Loader["InformationTitle"], flag)
                    {
                        Content = information
                    };
                settingPopup.Show();
            }
        }

        private void ShowSettingPopup(SettingSourceFlag flag = SettingSourceFlag.ByApp)
        {
            var settingPopup = new SettingPopup(ResourcesLoader.Loader["Setting"], flag)
                {
                    Content = new GlobalSetting()
                };
            settingPopup.Show();
        }

        private void ShowHelpPopup()
        {
            var help = new FullScreenPopup(ConstKeys.HELP_KEY, new HelpContent());
            help.Show();
        }
    }
}