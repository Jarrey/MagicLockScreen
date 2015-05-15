using MagicLockScreen_UI.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MagicLockScreen_UI
{
    public sealed partial class HelpContent : UserControl
    {
        public HelpContent()
        {
            InitializeComponent();
            DataContext = new HelpContentViewModel(this, null);
        }
    }
}