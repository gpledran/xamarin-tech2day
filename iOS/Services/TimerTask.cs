using System;
using System.Reactive.Linq;
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
        IDisposable timer;

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
			timer.Dispose();
        }
		
        void OnExpiration()
		{
			cancellationTokenSource.Cancel();
			timer.Dispose();
		}

		public async Task RunTimer(CancellationToken token)
		{
            notificationManager = new NotificationManager();

			await Task.Run(() =>
			{
				timer = Observable.Interval(TimeSpan.FromSeconds(1))
				.Subscribe(s =>
				{
                    token.ThrowIfCancellationRequested();

					var message = new ProgressMessage { Message = new DateTime(TimeSpan.FromSeconds(s).Ticks).ToString("mm:ss") };

					notificationManager.Show("Timer is running:", message.Message, NotificationIdServiceInProgress);

					MessagingCenter.Send(message, nameof(ProgressMessage));
				});
			}, token);
		}
    }
}
