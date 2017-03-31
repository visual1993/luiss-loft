using System;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

using CommonClasses;

using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace LoftServer
{
	public class EventREST: NancyModule
	{
		public EventREST()
		{
			Get[MainClass.LoftPrefix + "events"] = (dynamic arg) =>
			{
				return GetEvents();
			};
			Get[MainClass.LoftPrefix + "event/{guid}", runAsync: true] = async (dynamic parameters, System.Threading.CancellationToken arg2) =>
			 {
				 return HttpStatusCode.NotImplemented;
			 };
			Get[MainClass.LoftPrefix + "file/get/{fileid}"] = (dynamic arg) =>
			{
				string fileID = arg.fileid;
				var req = Generic.Drive.Files.Get(fileID);
				var res = req.Execute();

				string ContentType = "image/jpg";
				MemoryStream memory = new MemoryStream();
				req.Download(memory);
				memory.Position = 0;
				return Response.FromStream(memory, ContentType);
			};
		}

		public static string GetEvents()
		{ 
			GoogleEvent.EventsResponse output = new GoogleEvent.EventsResponse();

			EventsResource.ListRequest request = 
				Generic.Calendar.Events.List(MainClass.CalendarId); //primary //loft.luiss@gmail.com //visual1993@gmail.com
			request.TimeMin = DateTime.Now;
			request.ShowDeleted = false;
			request.SingleEvents = false;
			request.MaxResults = 20;
			request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

			// List events.
			Events events = request.Execute();
			if (events.Items != null && events.Items.Count > 0)
			{
				foreach (var eventItem in events.Items)
				{
					output.items.Add(eventItem.ToGoogleEvent());
				}
			}
			return JsonConvert.SerializeObject(output);
		}

	}

}
