using System;
using System.Collections.Generic;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_ImageSearchService.Models;
using MagicLockScreen_Service_ImageSearchService.UI.ViewModels;
using NoteOne_Core.Common;
using NoteOne_Core.Interaction;
using NoteOne_Core.Interfaces;
using NoteOne_Core.UI.Common;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MagicLockScreen_Service_ImageSearchService.UI.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ImageSearchServiceChannelPage : LayoutAwarePage, ISearchable
    {
        public static ImageSearchServiceChannelPage Current;

        public ImageSearchServiceChannelPage()
        {
            InitializeComponent();

            // This is a static public property that will allow downstream pages to get
            // a handle to the MainPage instance in order to call methods that are in this class
            Current = this;

            RefreshContent();
        }

        /// <summary>
        ///     refresh content
        /// </summary>
        public override void RefreshContent()
        {
            base.RefreshContent();

            // generate itemGridView for items and add this into host Grid panel
            var itemGridViewHost = FindName("itemGridViewHost") as Panel;
            // generate itemListView for items and add this into host Grid panel
            var itemListViewHost = FindName("itemListViewHost") as Panel;

            if (itemGridViewHost != null)
            {
                itemGridViewHost.Children.Clear();
                itemGridViewHost.Children.Add(GenerateItemGridView());
                itemListViewHost.Children.Clear();
                itemListViewHost.Children.Add(GenerateItemListView());
            }
        }

        /// <summary>
        ///     Populates the page with content passed during navigation.  Any saved state is also
        ///     provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">
        ///     The parameter value passed to
        ///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested.
        /// </param>
        /// <param name="pageState">
        ///     A dictionary of state preserved by this page during an earlier
        ///     session.  This will be null the first time a page is visited.
        /// </param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            /// Navigate from home page
            if (navigationParameter != null && CoreApplication.Properties.ContainsKey(navigationParameter.ToString()))
            {
                var parameters = CoreApplication.Properties[navigationParameter.ToString()] as object[];

                if (parameters != null && parameters[2] is ImageSearchServiceChannelModel)
                {
                    var model = parameters[2] as ImageSearchServiceChannelModel;
                    switch (model.SearchProviderType)
                    {
                        case SearchProvider.Baidu:
                            DefaultViewModel = new BaiduImageSearchServiceChannelPageViewModel(this, pageState);
                            break;
                        case SearchProvider.Google:
                            DefaultViewModel = new GoogleImageSearchServiceChannelPageViewModel(this, pageState);
                            break;
                        case SearchProvider.Flickr:
                            DefaultViewModel = new FlickrImageSearchServiceChannelPageViewModel(this, pageState);
                            break;
                        case SearchProvider.InfoDotCom:
                            DefaultViewModel = new InfoImageSearchServiceChannelPageViewModel(this, pageState);
                            break;
                    }
                    QueryData(model.Title);
                    CacheData();
                }
                return;
            }


            /// Navigate from other pages
            if (navigationParameter != null &&
                CoreApplication.Properties.ContainsKey(ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY))
            {
                var viewModel = GetCacheData() as ViewModelBase;
                if (viewModel != null) DefaultViewModel = viewModel;
                return;
            }


            /// Navigate from search contract
            string currentSearchProvider = AppSettings.Instance[AppSettings.CURRENT_SEARCH_PROVIDER].ToString();
            if (currentSearchProvider == SearchProvider.Baidu.GetDescription())
                DefaultViewModel = new BaiduImageSearchServiceChannelPageViewModel(this, pageState);
            else if (currentSearchProvider == SearchProvider.Google.GetDescription())
                DefaultViewModel = new GoogleImageSearchServiceChannelPageViewModel(this, pageState);
            else if (currentSearchProvider == SearchProvider.Flickr.GetDescription())
                DefaultViewModel = new FlickrImageSearchServiceChannelPageViewModel(this, pageState);
            else if (currentSearchProvider == SearchProvider.InfoDotCom.GetDescription())
                DefaultViewModel = new InfoImageSearchServiceChannelPageViewModel(this, pageState);

            CacheData();
        }

        /// <summary>
        ///     Preserves state associated with this page in case the application is suspended or the
        ///     page is discarded from the navigation cache.  Values must conform to the serialization
        ///     requirements of <see cref="SuspensionManager.SessionState" />.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            DefaultViewModel.SaveState(pageState);
        }

        #region generate UIElements via code

        private GridView _currentItemGridView;

        private ListView _currentItemListView;

        private GridView GenerateItemGridView()
        {
            if (_currentItemGridView != null)
            {
                _currentItemGridView.ClearValue(Interaction.TriggersProperty);
                _currentItemGridView.ClearValue(ItemsControl.ItemsSourceProperty);
                _currentItemGridView.ClearValue(ItemsControl.ItemTemplateProperty);
            }

            var itemGridView = new GridView
                {
                    Padding = new Thickness(116, 0, 116, 46),
                    SelectionMode = ListViewSelectionMode.None,
                    IsItemClickEnabled = true,
                    IsSwipeEnabled = false,
                    DataFetchSize = 0.4,
                    ItemTemplate = FindName("imageItemTemplate") as DataTemplate
                };
            itemGridView.SetValue(Interaction.TriggersProperty, FindName("itemGridViewTriggerCollection"));
            itemGridView.SetBinding(ItemsControl.ItemsSourceProperty,
                                    new Binding {Source = FindName("imageItemsViewSource") as CollectionViewSource});
            _currentItemGridView = itemGridView;
            return itemGridView;
        }

        private ListView GenerateItemListView()
        {
            if (_currentItemListView != null)
            {
                _currentItemListView.ClearValue(Interaction.TriggersProperty);
                _currentItemListView.ClearValue(ItemsControl.ItemsSourceProperty);
                _currentItemListView.ClearValue(ItemsControl.ItemTemplateProperty);
            }

            var itemListView = new ListView
                {
                    Padding = new Thickness(0, 0, 0, 60),
                    SelectionMode = ListViewSelectionMode.None,
                    IsItemClickEnabled = true,
                    IsSwipeEnabled = false,
                    DataFetchSize = 0.2,
                    ItemTemplate = FindName("imageItemTemplate") as DataTemplate
                };
            itemListView.SetValue(Interaction.TriggersProperty, FindName("itemGridViewTriggerCollection"));
            itemListView.SetBinding(ItemsControl.ItemsSourceProperty,
                                    new Binding {Source = FindName("imageItemsViewSource") as CollectionViewSource});
            _currentItemListView = itemListView;

            return itemListView;
        }

        #endregion

        #region Search data

        public void QueryData(string queryText)
        {
            var searchableViewModel = DefaultViewModel as ISearchable;
            if (searchableViewModel != null)
                searchableViewModel.QueryData(queryText);
        }

        #endregion

        #region Cache the current page data

        private void CacheData()
        {
            if (CoreApplication.Properties.ContainsKey(ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY))
                CoreApplication.Properties.Remove(ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY);
            CoreApplication.Properties.Add(ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY, DefaultViewModel);
        }

        private object GetCacheData()
        {
            if (CoreApplication.Properties.ContainsKey(ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY))
                return CoreApplication.Properties[ConstKeys.IMAGE_SEARCH_PAGE_CACHE_KEY];
            else return null;
        }

        #endregion
    }
}