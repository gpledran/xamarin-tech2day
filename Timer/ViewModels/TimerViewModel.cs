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
            HandleReceivedTickMessages();
        }

		void HandleReceivedTickMessages()
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
                var message = new StopMessage();
				MessagingCenter.Send(message, nameof(StopMessage));
                IsRunning = false;
				ButtonText = "Start";
            }
            else
            {
				var message = new StartMessage();
				MessagingCenter.Send(message, nameof(StartMessage));
                IsRunning = true;
                ButtonText = "Stop";
            }

            IsBusy = false;
        }
    }
}
