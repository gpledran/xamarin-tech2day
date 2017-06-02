using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Timer.Messages;
using Xamarin.Forms;

namespace Timer.Droid.Services
{
    [Service]
    public class TimerTask : Service
    {
        static int NotificationIdServiceInProgress = 17;

        CancellationTokenSource cancellationTokenSource;
        NotificationManager notificationManager;
        IDisposable timer;

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			cancellationTokenSource = new CancellationTokenSource();

			Task.Run(() =>
			{
				try
				{
                    RunTimer(cancellationTokenSource.Token).Wait();
				}
				catch (Android.Accounts.OperationCanceledException)
				{
				}
				finally
				{
                    if (cancellationTokenSource.IsCancellationRequested)
					{
                        notificationManager.Cancel(NotificationIdServiceInProgress);
					}
				}

            }, cancellationTokenSource.Token);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
            if (cancellationTokenSource != null)
            {                
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
                cancellationTokenSource.Cancel();
                timer.Dispose();
            }
			base.OnDestroy();
		}

		public async Task RunTimer(CancellationToken token)
		{
            NotificationCompat.Builder builder = GetNotificationBuilder();
            notificationManager = (NotificationManager)GetSystemService(NotificationService);

			await Task.Run(() =>
			{
				timer = Observable.Interval(TimeSpan.FromSeconds(1))
				.Subscribe(s =>
				{
					token.ThrowIfCancellationRequested();

					var progress = new ProgressMessage { Message = new DateTime(TimeSpan.FromSeconds(s).Ticks).ToString("mm:ss") };

					notificationManager.Notify(NotificationIdServiceInProgress, GetNotificationBuilder(progress.Message).Build());

					MessagingCenter.Send(progress, nameof(ProgressMessage));
				});
			}, token);
		}

		NotificationCompat.Builder GetNotificationBuilder(string content = "00:00")
		{
			return new NotificationCompat.Builder(this)
										 .SetSmallIcon(Resource.Drawable.icon)
										 .SetContentTitle("Timer is running:")
										 .SetContentText(content)
										 .SetOngoing(true);
		}

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}
