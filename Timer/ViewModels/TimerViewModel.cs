using System.Diagnostics;
using System.Windows.Input;
using MvvmHelpers;
using Timer.Messages;
using Xamarin.Forms;

namespace Timer.ViewModels
{
    public class TimerViewModel : BaseViewModel
    {
        public bool IsRunning { get; set; }
        public ICommand StartStopCommand { get; set; }

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
			MessagingCenter.Subscribe<TickMessage>(this, nameof(TickMessage), message =>
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
                var message = new StopTimerMessage();
				MessagingCenter.Send(message, nameof(StopTimerMessage));
                IsRunning = false;
				ButtonText = "Start";
            }
            else
            {
				var message = new StartTimerMessage();
				MessagingCenter.Send(message, nameof(StartTimerMessage));
                IsRunning = true;
                ButtonText = "Stop";
            }

            IsBusy = false;
        }
    }
}
