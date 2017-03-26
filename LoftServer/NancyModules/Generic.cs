using Google.Apis.Auth.OAuth2;

using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Nancy;
using Nancy.Bootstrapper;

using Newtonsoft.Json;

namespace LoftServer
{
	public class Generic
	{
		static string[] CalendarScopes = { CalendarService.Scope.CalendarReadonly };
		static string[] DriveScopes = { DriveService.Scope.DriveReadonly };
		static string ApplicationName = "Google Calendar API .NET Quickstart";

		public static CalendarService Calendar = null;
		public static DriveService Drive = null;

		public class Bootstrapper : DefaultNancyBootstrapper
		{
			protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
			{
				//CORS Enable
				pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
				{
					ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
									.WithHeader("Access-Control-Allow-Methods", "POST,GET")
									.WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

				});
				base.ApplicationStartup(container, pipelines);
			}

		}
		public Generic()
		{
			GoogleCalendarInit();
		}
		void GoogleCalendarInit()
		{
			UserCredential calendarCredential;
			UserCredential driveCredential;

			using (var stream =
				new FileStream("./client_secret_calendar.json", FileMode.Open, FileAccess.Read))
			{
				//string credPath = System.Environment.GetFolderPath(
					//System.Environment.SpecialFolder.Personal);
				//credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");
				var credPath = "./credentials/calendar-dotnet-quickstart.json";

				calendarCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					CalendarScopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}


			using (var stream =
				new FileStream("./client_secret_drive.json", FileMode.Open, FileAccess.Read))
			{
				//string credPath = System.Environment.GetFolderPath(
					//System.Environment.SpecialFolder.Personal);
				//credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");
				var credPath = "./credentials/drive-dotnet-quickstart.json";
				driveCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					DriveScopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}


			// Create Google Calendar API service.
			Calendar = new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = calendarCredential,
				ApplicationName = ApplicationName,
			});


			Drive = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = driveCredential,
				ApplicationName = ApplicationName,
			});



		}
	}
	public static class Extensions
	{
		public static CommonClasses.GoogleEvent ToGoogleEvent(this Event i)
		{
			var o = new CommonClasses.GoogleEvent();
			o.ID = i.Id;
			try
			{
				var rd = JsonConvert.DeserializeObject<CommonClasses.GoogleEvent.RichDescription>(o.Description);
				o.Description = rd.Description;
				o.ImageUris = rd.ImageUris;
			}
			catch {
				o.Description = i.Description;
			}
			o.Name = i.Summary;
			o.StartDate = i.Start.DateTime ?? DateTime.MinValue;
			o.EndDate = i.End.DateTime ?? DateTime.MinValue;

			#region ImageDeprecated

			if (i.Attachments != null)
			{
				foreach (var attachment in i.Attachments)
				{
					if ((attachment.MimeType ?? "").Contains("image/") || (attachment.Title ?? "").Contains(".png") || (attachment.Title ?? "").Contains(".jpg"))
					{
						o.ImageUris.Add(MainClass.ExternalAccessBaseUri+MainClass.LoftPrefix+"file/get/"+attachment.FileId);
						Debug.WriteLine("File id: "+attachment.FileId);
					}
				}
			}

			#endregion
			return o;
		}
	}
}
