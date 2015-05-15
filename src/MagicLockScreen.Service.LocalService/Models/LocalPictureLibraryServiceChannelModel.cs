using System.Collections.ObjectModel;
using MagicLockScreen_Helper.Models;
using MagicLockScreen_Service_LocalService.Resources;
using MagicLockScreen_Service_LocalService.UI.Views;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace MagicLockScreen_Service_LocalService.Models
{
    public class LocalPictureLibraryServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public LocalPictureLibraryServiceChannelModel(ServiceChannel channel, string path)
            : base(channel)
        {
            Index = -2;
            Path = path;
            Title = ResourcesLoader.Loader["ServiceChannelTitle"] + "  " +
                    System.IO.Path.GetFileNameWithoutExtension(path);
            SubTitle = ResourcesLoader.Loader["ServiceChannelSubTitle"] + "\n" +
                       ResourcesLoader.Loader["ServiceChannelTitle"] + " " + path;
            Logo = new Collection<BindableImage>
                {
                    new BindableImage
                        {
                            ThumbnailImageUrl = @"ms-appx:///Assets/ServiceChannelLogo//ChannelLogo2x2.png",
                            IsThumbnailImageDownloading = false
                        }
                };
            GroupID = ServiceChannelGroupID.LocalPictures;
            PrimaryViewType = typeof (LocalPictureLibraryServiceChannelPage);
            TempLogoFileName = path.GetUniqueFolderPath() + "_LPLLogo";
        }

        /// <summary>
        ///     tempary logo file name store in appdata/temp
        ///     for local picture library item to show the logo
        /// </summary>
        public string TempLogoFileName { get; set; }

        public override bool IsEnabled
        {
            get { return true; }
        }

        public RelayCommand ShowInfoCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        var information =
                            XamlReader.Load(ResourcesLoader.Loader["ServiceChannelInformation"]) as UIElement;
                        if (information != null)
                        {
                            var settingPopup = new SettingPopup(ResourcesLoader.Loader["InformationTitle"])
                                {
                                    Content = information
                                };
                            settingPopup.Show();
                        }
                    });
            }
        }
    }
}