using Timer.ViewModels;
using Xamarin.Forms;

namespace Timer.Views
{
    public partial class TimerPage : ContentPage
    {
        public TimerPage()
        {
            InitializeComponent();
			
            BindingContext = new TimerViewModel();
        }
    }
}
