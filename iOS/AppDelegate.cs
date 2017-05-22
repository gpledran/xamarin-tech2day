using Foundation;
using Timer.iOS.Services;
using Timer.Messages;
using UIKit;
using Xamarin.Forms;

namespace Timer.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            WireUpTimerTask();

            return base.FinishedLaunching(app, options);
        }

        TimerTask timerTask;
		void WireUpTimerTask()
		{
            MessagingCenter.Subscribe<StartTimerMessage>(this, nameof(StartTimerMessage), async message =>
			{
                timerTask = new TimerTask();
                await timerTask.Start();
			});

            MessagingCenter.Subscribe<StopTimerMessage>(this, nameof(StopTimerMessage), message =>
			{
				timerTask.Stop();
			});
		}
    }
}
