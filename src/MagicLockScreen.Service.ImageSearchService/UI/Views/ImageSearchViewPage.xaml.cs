﻿using System;
using System.Collections.Generic;
using NoteOne_Core.Common;
using NoteOne_Core.UI.Common;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MagicLockScreen_Service_ImageSearchService.UI.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ImageSearchViewPage : LayoutAwarePage
    {
        public ImageSearchViewPage()
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
            if (navigationParameter == null) Frame.GoBack();

            if (CoreApplication.Properties.ContainsKey(navigationParameter.ToString()))
            {
                object parameter = CoreApplication.Properties[navigationParameter.ToString()];
                if (parameter is object[])
                {
                    DefaultViewModel = (parameter as object[])[1] as ViewModelBase;
                    DefaultViewModel["ImageSelectedItem"] =
                        ((parameter as object[])[0] as ItemClickEventArgs).ClickedItem;
                }
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
        }
    }
}