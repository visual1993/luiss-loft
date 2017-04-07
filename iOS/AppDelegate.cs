using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using FFImageLoading;

using Syncfusion.SfCalendar.XForms.iOS;

namespace LuissLoft.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			var processorsCount = System.Environment.ProcessorCount;
			var config = new FFImageLoading.Config.Configuration()
			{
				//SchedulerMaxParallelTasks = Math.Max(2, processorsCount - 1),
				//HttpClient = new System.Net.Http.HttpClient(new Xamarin.Android.Net.AndroidClientHandler()),
			};
			ImageService.Instance.Initialize(config);
			new SfCalendarRenderer();
			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
