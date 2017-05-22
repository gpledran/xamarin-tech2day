using System;
using System.Threading;
using System.Threading.Tasks;
using MyOptimo.iOS;
using Timer.Messages;
using Timer.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Timer.iOS.Services
{
    public class TimerTask
    {
        static int NotificationIdServiceInProgress = 17;

		nint taskId;
        CancellationTokenSource cancellationTokenSource;
        NotificationManager notificationManager;

        public async Task Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            taskId = UIApplication.SharedApplication.BeginBackgroundTask(nameof(TimerTask), OnExpiration);

            notificationManager = new NotificationManager();

            try
            {
				MessagingCenter.Subscribe<TickMessage>(this, nameof(TickMessage), message =>
				{
                    notificationManager.Show("Timer is running:", message.Message, NotificationIdServiceInProgress);
				});

                var counter = new CounterTask();
                await counter.Run(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {}
            finally
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    var message = new CancelMessage();
                    Device.BeginInvokeOnMainThread(() => 
                    {
                        MessagingCenter.Send(message, nameof(CancelMessage));
                    });
                    MessagingCenter.Unsubscribe<CounterTask, TickMessage>(this, nameof(TickMessage));
                }
            }

            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
		
        void OnExpiration()
		{
            cancellationTokenSource.Cancel();
		}
    }
}
