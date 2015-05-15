using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Models;
using MagicLockScreen_Service_ImageSearchService.Resources;
using MagicLockScreen_Service_ImageSearchService.UI.Views;
using NoteOne_Core;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.UI.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace MagicLockScreen_Service_ImageSearchService.Models
{
    public class ImageSearchServiceChannelModel : MagicLockScreenServiceChannelModel
    {
        public ImageSearchServiceChannelModel(ServiceChannel channel, string title, string subTitle)
            : base(channel)
        {
            Index = -1;
            Title = title;
            SubTitle = subTitle;
            Logo = new Collection<BindableImage>
                {
                    new BindableImage
                        {
                            ThumbnailImageUrl = @"ms-appx:///Assets/ServiceChannelLogo//ChannelLogo2x2.png",
                            IsThumbnailImageDownloading = false
                        }
                };
            GroupID = ServiceChannelGroupID.CustomSearchPictures;
            PrimaryViewType = typeof (ImageSearchServiceChannelPage);
        }

        public SearchProvider SearchProviderType { get; set; }

        public RelayCommand ShowInfoCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        UIElement information = null;
                        switch(SearchProviderType)
                        {
                            case SearchProvider.Baidu:
                                information = XamlReader.Load(string.Format(
                                    ResourcesLoader.Loader["BaiduServiceChannelInformation"], Title)) as UIElement;
                                break;
                            case SearchProvider.Google:
                                information = XamlReader.Load(string.Format(
                                    ResourcesLoader.Loader["GoogleServiceChannelInformation"], Title)) as UIElement;
                                break;
                            case SearchProvider.Flickr:
                                information = XamlReader.Load(string.Format(
                                    ResourcesLoader.Loader["FlickrServiceChannelInformation"], Title)) as UIElement;
                                break;
                            case SearchProvider.InfoDotCom:
                                information = XamlReader.Load(string.Format(
                                    ResourcesLoader.Loader["InfoServiceChannelInformation"], Title)) as UIElement;
                                break;
                        }

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