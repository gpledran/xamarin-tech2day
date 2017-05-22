using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Timer.Messages;
using Timer.Tasks;
using Xamarin.Forms;

namespace Timer.Droid.Services
{
    [Service]
    public class TimerTask : Service
    {
        CancellationTokenSource cancellationTokenSource;

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			cancellationTokenSource = new CancellationTokenSource();

			Task.Run(() =>
			{
				try
				{
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

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}
