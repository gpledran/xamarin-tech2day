using System;
using Timer.Messages;
using Xamarin.Forms;

namespace Timer
{
    public partial class TimerPage : ContentPage
    {
        public TimerPage()
        {
            InitializeComponent();

            start.Clicked += HandleStartClicked;

            HandleReceivedTickMessages();
        }

        void HandleStartClicked(object sender, EventArgs e)
        {
            var message = new StartTimerMessage();
            MessagingCenter.Send(message, nameof(StartTimerMessage));
        }

        void HandleReceivedTickMessages()
        {
            MessagingCenter.Subscribe<TickMessage>(this, nameof(TickMessage), message => 
            {
                Device.BeginInvokeOnMainThread(() => 
                {
                    timer.Text = message.Message;    
                });
            });
        }
    }
}
