using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using FFImageLoading;

namespace LuissLoft.Droid
{
	[Activity(Label = "LuissLoft", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			var processorsCount = System.Environment.ProcessorCount;
			var config = new FFImageLoading.Config.Configuration()
			{
				SchedulerMaxParallelTasks = Math.Max(2, processorsCount - 1),
				HttpClient = new System.Net.Http.HttpClient(new Xamarin.Android.Net.AndroidClientHandler()),
			};
			ImageService.Instance.Initialize(config);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}
	}
}
