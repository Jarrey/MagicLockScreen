using System;
using System.Collections.Generic;
using MagicLockScreen_Service_FlickrService.UI.ViewModels;
using NoteOne_Core.Interaction;
using NoteOne_Core.UI.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MagicLockScreen_Service_FlickrService.UI.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FlickrServiceChannelPage : LayoutAwarePage
    {
        public FlickrServiceChannelPage()
        {
            InitializeComponent();
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
            DefaultViewModel =
                new FlickrServiceChannelPageViewModel(this, pageState);
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
                                    new Binding {Source = FindName("flickrItemsViewSource") as CollectionViewSource});
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
                                    new Binding {Source = FindName("flickrItemsViewSource") as CollectionViewSource});
            _currentItemListView = itemListView;

            return itemListView;
        }

        #endregion
    }
}