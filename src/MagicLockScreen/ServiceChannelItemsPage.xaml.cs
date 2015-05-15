using System;
using System.Collections.Generic;
using MagicLockScreen_Helper;
using MagicLockScreen_UI.ViewModels;
using NoteOne_Core.UI.Common;
using Windows.UI.Xaml.Controls;
using ConstKeys = NoteOne_Utility.ConstKeys;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace MagicLockScreen_UI
{
    /// <summary>
    ///     A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class ServiceChannelItemsPage : LayoutAwarePage
    {
        public ServiceChannelItemsPage()
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // For optimaizing the loading performence
            // move data initialization into main page
            if (!Bootstrapper.CurrentBootstrapper.IsDataInitialized)
            {
                await Bootstrapper.CurrentBootstrapper.InitializeData();
            }

            DefaultViewModel = new ServiceItemsPageViewModel(this, pageState);

            // Show Help guideline at first run time
            if ((bool) AppSettings.Instance[AppSettings.NEED_HELP])
            {
                var help = new FullScreenPopup(ConstKeys.HELP_KEY, new HelpContent());
                help.Show();
                AppSettings.Instance[AppSettings.NEED_HELP] = false;
                await NoteOne_Utility.AppSettings.SaveSettings(AppSettings.Instance);
            }

            // If main page init count can mod 10, show rate and review prompt
            await ApplicationHelper.CheckAndShowRateReviewPromptAsync();
        }
    }
}