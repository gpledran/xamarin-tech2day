using System;
using System.Threading;
using System.Threading.Tasks;
using MyOptimo.iOS;
using Timer.Messages;
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

            try
            {
                await RunTimer(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) {}

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

		public async Task RunTimer(CancellationToken token)
		{
            notificationManager = new NotificationManager();

			await Task.Run(async () =>
			{
				for (long i = 1; i < long.MaxValue; i++)
				{
					token.ThrowIfCancellationRequested();

					await Task.Delay(1000);

					var message = new ProgressMessage { Message = new DateTime(TimeSpan.FromSeconds(i).Ticks).ToString("mm:ss") };

					notificationManager.Show("Timer is running:", message.Message, NotificationIdServiceInProgress);

					Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, nameof(ProgressMessage)));
				}
			}, token);
		}
    }
}
