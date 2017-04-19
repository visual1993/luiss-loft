using System;
using Nancy.Hosting.Self;
using Nancy;
using System.Diagnostics;

using System.Configuration;

namespace LoftServer
{
	class MainClass
	{
		public const string LoftPrefix = "loft/";
		public static bool killProcess = false;
		public static string CalendarId = "h9kcedctrf2j9e4rqi0c40c7fg@group.calendar.google.com";
		public static string StudentsCalendar = "h9kcedctrf2j9e4rqi0c40c7fg@group.calendar.google.com";
#if DEBUG
		public const string ExternalAccessBaseUri = "http://127.0.0.1:8889/";
#else
		public const string ExternalAccessBaseUri = "http://rest.luiss.visual1993.com/";
#endif

		public static void Main(string[] args)
		{
			new Generic();

			var nancyConfig = new HostConfiguration
			{
				RewriteLocalhost = true
			};
			string uriString = "http://localhost:8889/";
#if DEBUG
			uriString = "http://127.0.0.1:8889/";
#endif
			using (var nancyHost = new NancyHost(nancyConfig, new Uri(uriString)))
			{
				StaticConfiguration.DisableErrorTraces = false;

				nancyHost.Start();

				Console.WriteLine("now listening on "+uriString+". Write exit to stop");

				while (!killProcess)
				{
					System.Threading.Thread.Sleep(150);
					waitForUserInput();
				}
			}

			Console.WriteLine("Stopped. Good bye!");
		}
		public static void waitForUserInput()
		{
			var newLine = Console.ReadLine();
			switch (newLine)
			{
				case "exit": { killProcess = true; Environment.Exit(0); break; }
				case "test":
					{
						DemoGoogleCal.Test();
						break;
					}
				case "get": { 
						Console.WriteLine(EventREST.GetEvents());
						break;
					}
			}
		}
	}
}
