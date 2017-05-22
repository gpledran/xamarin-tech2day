using System;
using System.Threading;
using System.Threading.Tasks;
using Timer.Messages;
using Xamarin.Forms;

namespace Timer.Tasks
{
    public class CounterTask
    {
        public async Task Run(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                for (long i = 1; i < long.MaxValue; i++)
                {
					token.ThrowIfCancellationRequested();
					
					await Task.Delay(1000);
					var message = new TickMessage
					{
                        Message = new DateTime(TimeSpan.FromSeconds(i).Ticks).ToString("mm:ss")
					};
                    					
					Device.BeginInvokeOnMainThread(() => 
					{
						MessagingCenter.Send(message, nameof(TickMessage));
					});
                }
            }, token);
        }
    }
}
