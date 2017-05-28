using System.Diagnostics;
using System.Windows.Input;
using MvvmHelpers;
using Timer.Messages;
using Xamarin.Forms;

namespace Timer.ViewModels
{
    public class TimerViewModel : BaseViewModel
    {
        public bool IsRunning { get; private set; }
        public ICommand StartStopCommand { get; private set; }

        string buttonText;
		public string ButtonText
		{
            get { return buttonText; }
            set { SetProperty(ref buttonText, value); }
		}

        string formattedTimer;
		public string FormattedTimer
		{
			get { return formattedTimer; }
			set { SetProperty(ref formattedTimer, value); }
		}

        public TimerViewModel()
        {
            ButtonText = "Start";
            FormattedTimer = "00:00";
            StartStopCommand = new Command(StartStopTimer);
            SubscibeToMessages();
        }

		void SubscibeToMessages()
		{
			MessagingCenter.Subscribe<ProgressMessage>(this, nameof(ProgressMessage), message =>
			{
				FormattedTimer = message.Message;
			});
		}

        void StartStopTimer()
        {
            if (IsBusy) return;

            IsBusy = true;

            if (IsRunning)
            {
				MessagingCenter.Send(new StopMessage(), nameof(StopMessage));
                IsRunning = false;
				ButtonText = "Start";
            }
            else
            {
				MessagingCenter.Send(new StartMessage(), nameof(StartMessage));
                IsRunning = true;
                ButtonText = "Stop";
            }

            IsBusy = false;
        }
    }
}
