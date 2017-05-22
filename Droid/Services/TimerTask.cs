using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Timer.Messages;
using Timer.Tasks;
using Xamarin.Forms;

namespace Timer.Droid.Services
{
    [Service]
    public class TimerTask : Service
    {
        static int NotificationIdServiceInProgress = 17;

        CancellationTokenSource cancellationTokenSource;
        NotificationManager notificationManager;

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			cancellationTokenSource = new CancellationTokenSource();

			NotificationCompat.Builder builder = GetNotificationBuilder();
			notificationManager = (NotificationManager)GetSystemService(NotificationService);

			Task.Run(() =>
			{
				try
				{
					MessagingCenter.Subscribe<TickMessage>(this, nameof(TickMessage), message =>
					{
						notificationManager.Notify(NotificationIdServiceInProgress,
												   GetNotificationBuilder(message.Message).Build());
					});

					var counter = new CounterTask();
                    counter.Run(cancellationTokenSource.Token).Wait();
				}
				catch (Android.Accounts.OperationCanceledException)
				{
				}
				finally
				{
                    if (cancellationTokenSource.IsCancellationRequested)
					{
						var message = new CancelMessage();
                        Device.BeginInvokeOnMainThread(() => {
                            MessagingCenter.Send(message, nameof(CancelMessage));
						});
						MessagingCenter.Unsubscribe<CounterTask, TickMessage>(this, nameof(TickMessage));
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
            }
			base.OnDestroy();
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
