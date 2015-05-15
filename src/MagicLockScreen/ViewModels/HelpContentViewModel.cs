using System.Collections.Generic;
using MagicLockScreen_UI.Resources;
using NoteOne_Core.Command;
using NoteOne_Core.Common;
using NoteOne_Core.UI.Common;
using NoteOne_Utility;
using Windows.UI.Xaml;

namespace MagicLockScreen_UI.ViewModels
{
    public class HelpContentViewModel : ViewModelBase
    {
        public HelpContentViewModel(FrameworkElement view, Dictionary<string, object> pageState)
            : base(view, pageState)
        {
            this["HelpPages"] = new List<string>
                {
                    ResourcesLoader.Loader["HelpPage1"],
                    ResourcesLoader.Loader["HelpPage2"],
                    ResourcesLoader.Loader["HelpPage3"]
                };

            #region Commands

            #region CloseHelpCommand

            this["CloseHelpCommand"] = new RelayCommand(() =>
                {
                    if (
                        FullScreenPopup.Current.ContainsKey(
                            ConstKeys.HELP_KEY))
                        FullScreenPopup.Current[ConstKeys.HELP_KEY].Close();
                });

            #endregion

            #endregion
        }

        public override void LoadState()
        {
        }

        public override void SaveState(Dictionary<string, object> pageState)
        {
        }
    }
}