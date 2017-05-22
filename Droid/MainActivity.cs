using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Timer.Droid.Services;
using Xamarin.Forms;
using Timer.Messages;

namespace Timer.Droid
{
    [Activity(Label = "Timer.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());

            WireUpTimerTask();
        }

		void WireUpTimerTask()
		{
			MessagingCenter.Subscribe<StartTimerMessage>(this, nameof(StartTimerMessage), message =>
			{
                var intent = new Intent(this, typeof(TimerTask));
				StartService(intent);
			});

			MessagingCenter.Subscribe<StopTimerMessage>(this, nameof(StopTimerMessage), message =>
			{
                var intent = new Intent(this, typeof(TimerTask));
				StopService(intent);
			});
		}
	}
}
