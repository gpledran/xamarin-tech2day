using System;
using System.Threading;
using System.Threading.Tasks;
using Timer.Messages;
using Timer.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Timer.iOS.Services
{
    public class TimerTask
    {
		nint taskId;
        CancellationTokenSource cancellationTokenSource;

        public async Task Start()
        {
            cancellationTokenSource = new CancellationTokenSource();

            taskId = UIApplication.SharedApplication.BeginBackgroundTask(nameof(TimerTask), OnExpiration);

            try
            {
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
