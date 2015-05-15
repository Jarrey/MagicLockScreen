using System;
using System.Collections.Generic;
using MagicLockScreen_Service_GooglePicasaService.Models;
using MagicLockScreen_Service_GooglePicasaService.UI.ViewModels;
using NoteOne_Core.Common;
using NoteOne_Core.Interaction;
using NoteOne_Core.UI.Common;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MagicLockScreen_Service_GooglePicasaService.UI.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GooglePicasaGroupViewPage : LayoutAwarePage
    {
        public GooglePicasaGroupViewPage()
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
            if (itemGridViewHost != null)
            {
                itemGridViewHost.Children.Clear();
                itemGridViewHost.Children.Add(GenerateItemGridView());
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
            if (navigationParameter == null) Frame.GoBack();

            if (CoreApplication.Properties.ContainsKey("NavigateGooglePicasaGroupViewPageKey"))
            {
                object parameter = CoreApplication.Properties["NavigateGooglePicasaGroupViewPageKey"];

                var viewModel =
                    new GooglePicasaGroupViewPageViewModel(this, pageState);
                if (parameter is GooglePicasaGroup)
                    viewModel["GooglePicasaSelectedGroup"] = parameter;
                DefaultViewModel = viewModel;
            }
            else
                Frame.GoBack();
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
                                    new Binding
                                        {
                                            Source = FindName("googlePicasaItemsViewSource") as CollectionViewSource
                                        });
            _currentItemGridView = itemGridView;
            return itemGridView;
        }

        #endregion
    }
}