using MagicLockScreen_Service_GeneralSettingService.UI.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MagicLockScreen_Service_GeneralSettingService.UI.Views
{
    public sealed partial class GlobalSetting : UserControl
    {
        public GlobalSetting()
        {
            InitializeComponent();
            DataContext = new GlobalSettingViewModel(this, null);
        }
    }
}