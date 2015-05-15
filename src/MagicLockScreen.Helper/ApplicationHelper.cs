using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MagicLockScreen_Helper.Resources;
using NoteOne_Core.Command;
using NoteOne_Core.Notifications.TileContent;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Buffer = Windows.Storage.Streams.Buffer;
using NoteOne_Core.Notifications;
using NoteOne_Utility.Helpers;

namespace MagicLockScreen_Helper
{
    public class ApplicationHelper
    {
        private static string EncryptedKey = MD5Encryptor.GetMD5("chameleon");
        private const string DesktopServiceAddressFile = "address.service";
        private const string DesktopServiceOKResponse = "Bomb";
        public static Dictionary<string, string> SupportImageExtensions = new Dictionary<string, string>();

        static ApplicationHelper()
        {
            SupportImageExtensions.Add("jpg", ".jpg");
            SupportImageExtensions.Add("png", ".png");
            SupportImageExtensions.Add("bmp", ".bmp");
            SupportImageExtensions.Add("jpeg", ".jpeg");
            SupportImageExtensions.Add("tif", ".tif");
            SupportImageExtensions.Add("gif", ".gif");
        }

        #region App Global Functions

        /// <summary>
        ///     Set Applciation Background Image
        /// </summary>
        /// <param name="imageUrl">The Image URL</param>
        /// <returns></returns>
        public static async Task SetAppBackgroundAsync(string imageUrl)
        {
            try
            {
                var bitmap = new BitmapImage();
                var imageBrush = Application.Current.Resources["appBackground"] as ImageBrush;
                if (imageBrush != null)
                    imageBrush.ImageSource = bitmap;
                StorageFile file =
                    await
                    ApplicationData.Current.RoamingFolder.CreateFileAsync("appbg.jpg",
                                                                          CreationCollisionOption.ReplaceExisting);
                await NoteOne_Utility.Helpers.ApplicationHelper.StoreImageAsync(imageUrl, file);
                bitmap.UriSource = new Uri(@"ms-appdata:///roaming/appbg.jpg");
                new MessagePopup(ResourcesLoader.Loader["SetAppBgSucessfully"]).Show();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                new MessagePopup(ResourcesLoader.Loader["SetAppBgError"]).Show();
            }
        }

        /// <summary>
        ///     Set lock screen background from image url
        /// </summary>
        /// <param name="imageUrl">image url</param>
        /// <returns></returns>
        public static async Task SetLockScreenBackgroundAsync(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                if (uri.Scheme == "file")
                {
                    StorageFile imagFile = await StorageFile.GetFileFromPathAsync(imageUrl);
                    using (IRandomAccessStreamWithContentType s = await imagFile.OpenReadAsync())
                    {
                        await LockScreen.SetImageStreamAsync(await imagFile.OpenReadAsync());
                    }
                }
                else
                {
                    RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromUri(new Uri(imageUrl));
                    using (IRandomAccessStreamWithContentType s = await stream.OpenReadAsync())
                    {
                        await LockScreen.SetImageStreamAsync(s);
                    }
                }
                new MessagePopup(ResourcesLoader.Loader["SetLockScreenSucessfully"]).Show();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                new MessagePopup(ResourcesLoader.Loader["SetLockScreenError"]).Show();
            }
        }

        /// <summary>
        ///     Set desktop wallpaper from image url
        /// </summary>
        /// <param name="imageUrl">image url</param>
        /// <returns></returns>
        public static async Task SetWallpaperAsync(string imageUrl, bool withUi = true)
        {
            try
            {
                var imageBase64 = string.Empty;
                var uri = new Uri(imageUrl);
                if (uri.Scheme == "file")
                {
                    StorageFile imagFile = await StorageFile.GetFileFromPathAsync(imageUrl);
                    using (IRandomAccessStreamWithContentType s = await imagFile.OpenReadAsync())
                    {
                        await SetWallpaperAsync(s, withUi);
                    }
                }
                else
                {
                    RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromUri(new Uri(imageUrl));
                    using (IRandomAccessStreamWithContentType s = await stream.OpenReadAsync())
                    {
                        await SetWallpaperAsync(s, withUi);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        /// <summary>
        ///     Set desktop wallpaper from image stream
        /// </summary>
        /// <param name="stream">image stream</param>
        /// <returns></returns>
        public static async Task SetWallpaperAsync(IRandomAccessStream stream, bool withUi = true)
        {
            try
            {
                stream.Seek(0);
                var imageBase64 = Convert.ToBase64String(await stream.GetBytes());
                var serviceaddress = await GetDesktopServiceAddress();
                await HttpClientHelper.Instance.PostResponseStringAsync(new Uri(serviceaddress + "/api/update?key=" + EncryptedKey), imageBase64);

                if (withUi)
                {
                    new MessagePopup(ResourcesLoader.Loader["SetWallpaperSucessfully"]).Show();
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                if (withUi)
                {
                    new MessagePopup(ResourcesLoader.Loader["SetWallpaperError"]).Show();
                }
            }
        }

        public static async Task<bool> CheckPromptDesktopService()
        {
            var result = await CheckDesktopService();
            if (!result)
            {
                string downloadUrl = LocalAppSettings.Instance[LocalAppSettings.DESKTOP_SERVICE_DOWNLOAD_URL].ToString();
                var rateReviewPrompt = new MessageDialog(string.Format(ResourcesLoader.Loader["ErrorOnDesktopService"], downloadUrl),
                                                         ResourcesLoader.Loader["ErrorOnDesktopServiceTitle"]);
                rateReviewPrompt.Commands.Add(new UICommand(ResourcesLoader.Loader["GoToDownload"], null, 1));
                rateReviewPrompt.Commands.Add(new UICommand(ResourcesLoader.Loader["Never"], null, 0));
                IUICommand command = await rateReviewPrompt.ShowAsync();

                if ((int)command.Id == 1) // choose download
                {
                    await Launcher.LaunchUriAsync(new Uri(downloadUrl));
                }
            }

            return result;
        }

        public static async Task<bool> CheckDesktopService()
        {
            try
            {
                var serviceaddress = await GetDesktopServiceAddress();
                if (string.IsNullOrEmpty(serviceaddress))
                    return false;

                var response = await HttpClientHelper.Instance.GetResponseStringAsync(new Uri(serviceaddress + "/api/IsValid?key=" + EncryptedKey));

                return DesktopServiceOKResponse == response.Body;
            }
            catch (Exception ex)
            {
                return false;
                ex.WriteLog();
            }
        }

        public static async Task<StorageFile> GetTemporaryStorageImageAsync(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            if (uri.Scheme == "file")
            {
                return await StorageFile.GetFileFromPathAsync(imageUrl);
            }
            else
            {
                RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromUri(uri);
                StorageFile imageFile =
                    await
                    ApplicationData.Current.TemporaryFolder.CreateFileAsync("storageimg",
                                                                            CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream randomAccessStream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Buffer buffer;
                    using (IRandomAccessStream originalImageStream = await stream.OpenReadAsync())
                    {
                        buffer = new Buffer((uint)originalImageStream.Size);
                        await
                            originalImageStream.ReadAsync(buffer, (uint)originalImageStream.Size,
                                                          InputStreamOptions.None);
                    }
                    await randomAccessStream.WriteAsync(buffer);
                    await randomAccessStream.FlushAsync();
                }
                return imageFile;
            }
        }

        /// <summary>
        ///     update the app tile
        /// </summary>
        /// <param name="url">image url</param>
        /// <param name="title1">primary title</param>
        /// <param name="title2">image description</param>
        public static void UpdateTileNotification(string url, string title1, string title2)
        {
            if (title1 != null && title1.StartsWith("\0")) title1 = " ";
            if (title2 != null && title2.StartsWith("\0")) title2 = " ";

            ITileWide310x150ImageAndText02 tileContent = TileContentFactory.CreateTileWide310x150ImageAndText02();
            tileContent.TextCaption1.Text = title1;
            tileContent.TextCaption2.Text = title2;
            tileContent.Image.Src = url;
            tileContent.Image.Alt = title1;
            ITileSquare150x150PeekImageAndText02 squareContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
            squareContent.Image.Src = url;
            squareContent.Image.Alt = title1;
            squareContent.TextHeading.Text = title1;
            squareContent.TextBodyWrap.Text = title2;
            tileContent.Square150x150Content = squareContent;
            TileNotification notification = tileContent.CreateNotification();
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        /// <summary>
        ///     Check app run count and show prompt let user to rate and review app in Microsoft Store
        /// </summary>
        /// <returns></returns>
        public static IAsyncAction CheckAndShowRateReviewPromptAsync()
        {
            return AsyncInfo.Run(async token =>
                {
                    int initCount =
                        LocalAppSettings.Instance[LocalAppSettings.INIT_MAIN_PAGE_COUNT].ToString().StringToInt();
                    if (initCount >= 0)
                    {
                        if (initCount > 10 && initCount % 10 == 0)
                        {
                            var rateReviewPrompt = new MessageDialog(ResourcesLoader.Loader["ReviewPromptContent"],
                                                                     ResourcesLoader.Loader["ReviewPromptTitle"]);
                            rateReviewPrompt.Commands.Add(new UICommand(ResourcesLoader.Loader["Rate"], null, 2));
                            rateReviewPrompt.Commands.Add(new UICommand(ResourcesLoader.Loader["Never"], null, 1));
                            rateReviewPrompt.Commands.Add(new UICommand(ResourcesLoader.Loader["Later"], null, 0));
                            IUICommand command = await rateReviewPrompt.ShowAsync();

                            if ((int)command.Id == 1) // choose never
                            {
                                LocalAppSettings.Instance[LocalAppSettings.INIT_MAIN_PAGE_COUNT] = -1;
                                await NoteOne_Utility.AppSettings.SaveSettings(LocalAppSettings.Instance);
                                return;
                            }

                            if ((int)command.Id == 2) // choose rate it
                            {
                                LocalAppSettings.Instance[LocalAppSettings.INIT_MAIN_PAGE_COUNT] = -1;
                                var uri = new Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName);
                                await Launcher.LaunchUriAsync(uri);
                                await NoteOne_Utility.AppSettings.SaveSettings(LocalAppSettings.Instance);
                                return;
                            }
                        }
                        LocalAppSettings.Instance[LocalAppSettings.INIT_MAIN_PAGE_COUNT] = initCount + 1;
                        await NoteOne_Utility.AppSettings.SaveSettings(LocalAppSettings.Instance);
                    }
                });
        }

        #region Desktop service helper

        private static async Task<string> GetDesktopServiceAddress()
        {
            try
            {
                if (!await ApplicationData.Current.LocalFolder.CheckFileExisted(DesktopServiceAddressFile))
                    return null;

                return await FileIO.ReadTextAsync(await ApplicationData.Current.LocalFolder.GetFileAsync(DesktopServiceAddressFile));
            }
            catch (Exception ex)
            {
                return null;
                ex.WriteLog();
            }
        }

        #endregion

        #endregion

        #region App Global Commands

        public static readonly RelayCommand<string> SetAsLockScreenCommand =
            new RelayCommand<string>(async url => { await SetLockScreenBackgroundAsync(url); });

        public static readonly RelayCommand<string> SetAppBackgroundCommand =
            new RelayCommand<string>(async url => { await SetAppBackgroundAsync(url); });

        public static readonly RelayCommand<string> SetWallpaperCommand =
            new RelayCommand<string>(async url =>
            {
                if (await CheckPromptDesktopService())
                {
                    await SetWallpaperAsync(url);
                }
            });

        public static readonly RelayCommand<string> SaveAsCommand = new RelayCommand<string>(async url =>
            {
                try
                {
                    string fileName = Regex.Match(url, @"\w*\.[\w]{2,4}$").Value;
                    string fileExtension = Regex.Match(fileName, @"\.[\w]{2,4}$").Value;
                    if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(fileExtension))
                    {
                        var fsp = new FileSavePicker
                            {
                                SuggestedStartLocation = (PickerLocationId)LocalAppSettings.Instance[LocalAppSettings.DEFAULT_IMAGE_SAVE_POSITION],
                                SuggestedFileName = fileName,
                                DefaultFileExtension = fileExtension
                            };
                        foreach (var fileType in SupportImageExtensions)
                        {
                            fsp.FileTypeChoices.Add(fileType.Key, new List<string> { fileType.Value });
                        }
                        StorageFile file = await fsp.PickSaveFileAsync();
                        if (file != null)
                        {
                            await NoteOne_Utility.Helpers.ApplicationHelper.StoreImageAsync(url, file);
                            new MessagePopup(ResourcesLoader.Loader["SaveSucessfully"] + file.Path).Show();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.WriteLog();
                    new MessagePopup(ResourcesLoader.Loader["SaveError"]).Show();
                }
            });

        #endregion
    }
}