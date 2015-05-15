using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_ImageSearchService.UI.Views;
using MagicLockScreen_UI.Resources;
using NoteOne_Core.Common;
using NoteOne_Core.Interfaces;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AppSettings = NoteOne_Utility.AppSettings;

namespace MagicLockScreen_UI
{
    public class Bootstrapper
    {
        // "3C0BCF48-361A-47A8-8DD1-4DFEC28F3E05" : v1.1
        // "29C813BF-9C3F-4A65-96C9-874E3D4B5562" : v1.2
        // "611483D9-510A-46C9-B339-41085CC5806C" : v1.3, v2.1
        private const string upgradeKey = "611483D9-510A-46C9-B339-41085CC5806C";

        private static Bootstrapper _currentInstance;

        /// <summary>
        ///     Searching page type
        /// </summary>
        private readonly Type searchCopntractPage = typeof(ImageSearchServiceChannelPage);

        private LaunchActivatedEventArgs launchArgs;
        private Frame rootFrame;
        private SearchActivatedEventArgs searchArgs;


        private Bootstrapper()
        {
        }

        public static Bootstrapper CurrentBootstrapper
        {
            get
            {
                if (_currentInstance == null)
                    _currentInstance = new Bootstrapper();
                return _currentInstance;
            }
        }

        public void Run(LaunchActivatedEventArgs args)
        {
            launchArgs = args;
            if (Window.Current.Content == null)
            {
                rootFrame = new Frame();
                rootFrame.Navigate(typeof(Splash), args.SplashScreen);
                Window.Current.Content = rootFrame;
            }
            Window.Current.Activate();
        }

        public async void RunBySearchActived(SearchActivatedEventArgs args)
        {
            searchArgs = args;

            if (!CurrentBootstrapper.IsDataInitialized)
                await CurrentBootstrapper.InitializeData();

            // To clean the image search page cache data
            if (CoreApplication.Properties.ContainsKey(ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY))
                CoreApplication.Properties.Remove(ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY);

            if (Window.Current.Content == null ||
                Window.Current.Content.GetType().FullName != searchCopntractPage.FullName)
            {
                rootFrame = new Frame();
                rootFrame.Navigate(searchCopntractPage, args);
                Window.Current.Content = rootFrame;
            }
            Window.Current.Activate();

            if (!string.IsNullOrEmpty(args.QueryText))
            {
                // display search results
                var frame = Window.Current.Content as Frame;
                if (frame != null)
                {
                    var searchPage = frame.Content as ISearchable;
                    if (searchPage != null)
                        searchPage.QueryData(args.QueryText);
                }
            }
        }

        public async Task Suspending()
        {
            try
            {
                await SuspensionManager.SaveAsync();
            }
            catch (SuspensionManagerException e)
            {
                e.WriteLog();
            }
            finally
            {
                NetworkStatusMonitor.CurrentNetworkStatusMonitor.UnRegisterForNetworkStatusChangeNotif();
                LogExtension.DestroyLogger();
            }
        }

        public async void LoadingCompleted()
        {
            await InitializeAppBackgroundAsync();
            await rootFrame.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                async () =>
                {
                    if (launchArgs.PreviousExecutionState == ApplicationExecutionState.Running)
                    {
                        CoreApplication.Properties.Clear();
                        Window.Current.Activate();
                        return;
                    }

                    rootFrame = new Frame();
                    SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                    if (launchArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        // Restore the saved session state only when appropriate
                        try
                        {
                            await InitializeData();
                            await SuspensionManager.RestoreAsync();
                        }
                        catch (SuspensionManagerException ex)
                        {
                            ex.WriteLog();
                        }
                    }

                    if (rootFrame.Content == null)
                    {
                        if (!rootFrame.Navigate(typeof(ServiceChannelItemsPage)))
                        {
                            new Exception("Failed to create initial page").WriteLog();
                        }
                    }
                    Window.Current.Content = rootFrame;
                    TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                });
        }

        #region Cleanup Method

        /// <summary>
        ///     Used for cleanup old data after app upgrading
        /// </summary>
        /// <returns></returns>
        private async Task CleanUpAsync()
        {
            await Task.Delay(10);
            // TODO: Implement custom cleanup logical here

            //foreach (StorageFile file in await ApplicationData.Current.LocalFolder.GetFilesAsync())
            //    await file.DeleteAsync();
            //foreach (StorageFile file in await ApplicationData.Current.RoamingFolder.GetStorageFiles(new string[] { ".setting" }, FolderDepth.Shallow))
            //    await file.DeleteAsync();

            LocalAppSettings.Instance.Reset();
            AppSettings.Instance.Reset();
            MagicLockScreen_Helper.AppSettings.Instance.Reset();

            // Initialize local aplication settings
            await AppSettings.InitializeSettings(LocalAppSettings.Instance);

            // Initialize roaming aplication settings
            await AppSettings.InitializeSettings(AppSettings.Instance);
            await AppSettings.InitializeSettings(MagicLockScreen_Helper.AppSettings.Instance);
            
            // unregister all UpdateLockScreenBackgroundTask tasks from previous version
            BackgroundTaskController.UnregisterBackgroundTasks("UpdateLockScreenBackgroundTask");
            new MessagePopup(ResourcesLoader.Loader["UpgradePrompt"]).Show(30);
        }

        #endregion

        #region Properties

        public bool IsDataInitialized { get; private set; }

        #endregion

        #region Initialize Data

        private IAsyncAction InitializeConfigAsync()
        {
            return AsyncInfo.Run(async token =>
                {
                    try
                    {
                        StorageFileQueryResult queryResult = Package.Current.InstalledLocation.
                                                                     CreateFileQueryWithOptions(
                                                                         new QueryOptions(CommonFileQuery.OrderByName,
                                                                                          new List<string> { ".mf" })
                                                                             {
                                                                                 FolderDepth = FolderDepth.Deep
                                                                             });


                        foreach (StorageFile file in await queryResult.GetFilesAsync())
                        {
                            // Load the service channels from config files
                            XmlDocument configXml = await XmlDocument.LoadFromFileAsync(file);
                            foreach (XmlElement s in configXml.GetElementsByTagName("ServiceChannel"))
                            {
                                Activator.CreateInstance(s.GetAttribute("Type").CheckAndThrow().GenerateType(), s);
                            }

                            // Load the known types
                            XmlNodeList knownTypes = configXml.GetElementsByTagName("KnownTypes");
                            if (knownTypes != null && knownTypes.Count > 0)
                            {
                                IXmlNode node = knownTypes.Item(0);
                                if (node != null)
                                    foreach (XmlElement assemblyElement in node.SelectNodes("Type"))
                                    {
                                        Type knownType = assemblyElement.GetAttribute("Name").GenerateType();
                                        if (!SuspensionManager.KnownTypes.Contains(knownType))
                                            SuspensionManager.KnownTypes.Add(knownType);
                                    }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                });
        }

        private IAsyncAction InitializeAppBackgroundAsync()
        {
            return AsyncInfo.Run(async token =>
                {
                    try
                    {
                        if (!(await ApplicationData.Current.RoamingFolder.CheckFileExisted("appbg.jpg")))
                        {
                            StorageFile appbgFile =
                                await
                                ApplicationData.Current.RoamingFolder.CreateFileAsync("appbg.jpg",
                                                                                      CreationCollisionOption
                                                                                          .ReplaceExisting);
                            StorageFile file =
                                await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/appbg.jpg"));
                            await file.CopyAndReplaceAsync(appbgFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
        }

        public IAsyncAction InitializeData()
        {
            return AsyncInfo.Run(async token =>
                {
                    // Cleanup temp folder files
                    foreach (IStorageItem item in await ApplicationData.Current.TemporaryFolder.GetItemsAsync())
                        await item.DeleteAsync();

                    // Initialize local aplication settings
                    await AppSettings.InitializeSettings(LocalAppSettings.Instance);

                    // Initialize roaming aplication settings
                    await AppSettings.InitializeSettings(AppSettings.Instance);
                    await AppSettings.InitializeSettings(MagicLockScreen_Helper.AppSettings.Instance);

                    // Check upgrade key to do cleanup after upgrading
                    if (LocalAppSettings.Instance[LocalAppSettings.UPGRADE_KEY].ToString() != upgradeKey)
                    {
                        await CleanUpAsync();
                        LocalAppSettings.Instance[LocalAppSettings.UPGRADE_KEY] = upgradeKey;
                        await AppSettings.SaveSettings(LocalAppSettings.Instance);
                    }

                    // Initialize log sub-system
                    await LogExtension.InitializeLogger();

                    // Initialize network monitor
                    await NetworkStatusMonitor.CurrentNetworkStatusMonitor.CheckInternetStatusAsync();
                    NetworkStatusMonitor.CurrentNetworkStatusMonitor.RegisterForNetworkStatusChangeNotif();

                    // Read all *.mf config files to initialize all service channels and services
                    await InitializeConfigAsync();

                    // Complete data initialization
                    IsDataInitialized = true;
                });
        }

        #endregion
    }
}