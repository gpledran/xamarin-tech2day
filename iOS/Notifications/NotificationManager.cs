using System;
using UIKit;
using UserNotifications;
using Foundation;

namespace MyOptimo.iOS
{
	public class NotificationManager
	{
		public const string NotificationKey = "NotificationKey";

		public void Show(string title, string body, int id = 0)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(.1, false);
				ShowUserNotification(title, body, id, trigger);
			}
			else
			{
				Show(title, body, id, DateTime.Now);
			}
		}

		public void Show(string title, string body, int id, DateTime notifyTime)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				var trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponentsFromDateTime(notifyTime), false);
				ShowUserNotification(title, body, id, trigger);
			}
			else
			{
				var notification = new UILocalNotification
				{
					FireDate = (NSDate)notifyTime,
					AlertTitle = title,
					AlertBody = body,
					UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey))
				};

				UIApplication.SharedApplication.ScheduleLocalNotification(notification);
			}
		}

		void ShowUserNotification(string title, string body, int id, UNNotificationTrigger trigger)
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				return;
			}

			var content = new UNMutableNotificationContent()
			{
				Title = title,
				Body = body,
				UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey))
			};

			var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);

			UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) => { });
		}

		NSDateComponents GetNSDateComponentsFromDateTime(DateTime dateTime)
		{
			return new NSDateComponents
			{
				Month = dateTime.Month,
				Day = dateTime.Day,
				Year = dateTime.Year,
				Hour = dateTime.Hour,
				Minute = dateTime.Minute,
				Second = dateTime.Second
			};
		}
	}
}
