using System;
using UserNotifications;
using System.Diagnostics;
using Xamarin.Forms;

namespace MyOptimo.iOS
{
	public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
	{
		public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
		{
            completionHandler(UNNotificationPresentationOptions.None);
		}

		public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
		{
			Debug.WriteLine("DidReceiveNotificationResponse");
			if (response.IsDefaultAction)
			{
				Debug.WriteLine("DidReceiveNotificationResponse");
			}
		}
	}
}