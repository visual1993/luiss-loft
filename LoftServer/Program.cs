using System;
using Nancy.Hosting.Self;
using Nancy;
using System.Diagnostics;
using System.Collections.Generic;

using Plugin.Visual1993.Gateway.Abstractions;

using System.Configuration;

namespace LoftServer
{
	class MainClass
	{
		public const string LoftPrefix = "loft/";
		public static bool killProcess = false;
		public static string CalendarId = "h9kcedctrf2j9e4rqi0c40c7fg@group.calendar.google.com";
		public static string StudentsCalendar = "hinu0p05iamc5en90j2llnds54@group.calendar.google.com";

		public const string Visual1993RestServer = "http://rest.visual1993.com/";
		//public const string Visual1993RestServer = "http://127.0.0.1:8888/";

		public const string StaffMail = "loft.luiss@gmail.com";
		//public const string StaffMail = "visual1993@gmail.com";

		//public static List<string> StaffCcMail = new List<string>{ "visual1993@gmail.com" }; //impostare a null se non si vuole usare
		public static List<string> StaffCcMail = null;
#if DEBUG
		public const string ExternalAccessBaseUri = "http://127.0.0.1:8889/";
#else
		public const string ExternalAccessBaseUri = "http://rest.luiss.visual1993.com/";
#endif

		public static void Main(string[] args)
		{
			new Generic();
			SetupGateway();
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
		private static void SetupGateway()
		{
			Constants.AppTokens = new List<string> {
			"e2fc341b-5185-410f-8adb-5006a9a3f616",
			"0b0eb8a7-01ae-4556-913b-0cc51b6a8fc6",
			"78a9fc3e-9a72-4246-927a-92689cd86c70"
			};
			Constants.RestAPI = "https://ssl.visual1993.com/luissloft/v1/";
			Constants.GatewayUrl = Constants.RestAPI + "gateway.php";
			Constants.GatewaySecureBlowfish = "luissloft";
		}
		public static void waitForUserInput()
		{
			var newLine = Console.ReadLine();
			switch (newLine)
			{
				case "exit": { killProcess = true; Environment.Exit(0); break; }
				case "test":
					{
						InternalEvent.getAll().ContinueWith((obj) => {
							var res = obj.Result;	
						});
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
