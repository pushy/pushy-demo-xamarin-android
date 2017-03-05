using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Support.V4.Content;

namespace PushyDemo
{
	[BroadcastReceiver(Enabled = true, Exported = false)]
	[IntentFilter(new[] { "pushy.me" })]
	public class PushReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			// Default notification title/text
			string notificationTitle = "Pushy";
			string notificationText = "Test notification";

			// Attempt to extract the "message" property from the payload: {"message":"Hello World!"}
			if (intent.GetStringExtra("message") != null)
			{
				notificationText = intent.GetStringExtra("message");
			}

			// Prepare a notification with vibration, sound and lights
			var notificationBuilder = new Notification.Builder(context)
                   	.SetSmallIcon(Resource.Mipmap.Notification)
					.SetContentTitle(notificationTitle)
					.SetContentText(notificationText)
					.SetLights(Color.Red, 1000, 1000)
					.SetVibrate(new long[] { 0, 400, 250, 400 })
				    .SetColor(ContextCompat.GetColor(context, Resource.Color.colorPrimary))
				   	.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
					.SetContentIntent(PendingIntent.GetActivity(context, 0, new Intent(context, typeof(MainActivity)), PendingIntentFlags.UpdateCurrent));

			// Get an instance of the NotificationManager service
			var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

			// Build the notification and display it
			notificationManager.Notify(1, notificationBuilder.Build());

		}
	}
}
