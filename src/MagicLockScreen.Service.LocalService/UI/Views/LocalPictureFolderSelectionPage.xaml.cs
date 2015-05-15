using System;
using System.Collections.Generic;
using MagicLockScreen_Service_LocalService.UI.ViewModels;
using NoteOne_Core.UI.Common;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace MagicLockScreen_Service_LocalService.UI.Views
{
    /// <summary>
    ///     This page displays folders in picture library
    /// </summary>
    public sealed partial class LocalPictureFolderSelectionPage : LayoutAwarePage
    {
        public LocalPictureFolderSelectionPage()
        {
            InitializeComponent();
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
                new LocalPictureFolderSelectionPageViewModel(this, pageState);
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
    }
}