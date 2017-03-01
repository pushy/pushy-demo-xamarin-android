using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content.PM;

using System.Threading.Tasks;
using Android.Support.V7.App;

using ME.Pushy.Sdk;
using ME.Pushy.Sdk.Util.Exceptions;

namespace PushyDemo
{
	[Activity(
		MainLauncher = true, 
		Icon = "@mipmap/icon",
		Label = "@string/appName", 
		LaunchMode = LaunchMode.SingleTask,
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard | ConfigChanges.ScreenSize | ConfigChanges.Locale)
	)]
	public class MainActivity : AppCompatActivity
	{
		TextView DeviceToken;
		TextView Instructions;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Load Main.axml layout
			SetContentView(Resource.Layout.Main);

			// Cache TextView objects
			DeviceToken = FindViewById<TextView>(Resource.Id.deviceToken);
			Instructions = FindViewById<TextView>(Resource.Id.instructions);

			// Restart the socket service, in case the user force-closed the app
			Pushy.Listen(this);

			// Register device for push notifications (async)
			await RegisterForPushNotifications();
		}

		private async Task RegisterForPushNotifications()
		{
			// Create progress dialog and set it up
			var loading = new ProgressDialog(this);
			loading.SetMessage(GetString(Resource.String.registeringDevice));
			loading.SetCancelable(false);

			// Show it
			loading.Show();

			// Register device for push notifications (async)
			var result = await RegisterForPushNotificationsAsync();

			// Activity died?
			if (IsFinishing)
			{
				return;
			}

			// Hide progress bar
			loading.Dismiss();

			// Registration failed?
			if (result.Error != null)
			{
				// Write error to logcat
				Log.Error("Pushy", "Registration failed: " + result.Error.Message);

				// Display registration failed in app UI
				Instructions.SetText(Resource.String.restartApp);
				DeviceToken.SetText(Resource.String.registrationFailed);

				// Display error dialog
				new Android.App.AlertDialog.Builder(this)
			           	.SetTitle(Resource.String.registrationError)
						.SetMessage(result.Error.Message)
					   	.SetPositiveButton(Resource.String.ok, (sender, e) => {})
						.Create()
						.Show();
			}
			else
			{
				// Write device token to logcat
				Log.Debug("Pushy", "Device token: " + result.DeviceToken);

				// Display device token and copy from logcat instructions
				DeviceToken.Text = result.DeviceToken;
				Instructions.SetText(Resource.String.copyLogcat);
			}
		}

		private async Task<RegistrationResult> RegisterForPushNotificationsAsync()
		{
			// Prepare registration result
			var result = new RegistrationResult();

			// Execute Pushy.Register() in a background thread
			await Task.Run(() =>
			{
				try
				{
					// Register device for push notifications
					result.DeviceToken = Pushy.Register(this);
				}
				catch (PushyException exc)
				{
					// Store registration error in result
					result.Error = exc;
				}

			});

			// Return registration error / device token
			return result;
		}
	}
}