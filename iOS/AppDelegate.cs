using Foundation;
using MyOptimo.iOS;
using Timer.iOS.Services;
using Timer.Messages;
using UIKit;
using UserNotifications;
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
            WireUpNotifications();


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

		void WireUpNotifications()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				UNUserNotificationCenter.Current.RequestAuthorization(
						UNAuthorizationOptions.Alert,
						(approved, error) => { });
                
				UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
			}
			else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var settings = UIUserNotificationSettings.GetSettingsForTypes(
						UIUserNotificationType.Alert,
						new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			}
		}
    }
}
