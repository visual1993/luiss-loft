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
using Visual1993.Data;

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
			Get[MainClass.LoftPrefix + "event/{guid}/accept", runAsync: true] = async (dynamic parameters, System.Threading.CancellationToken arg2) =>
			 {
				 return HttpStatusCode.NotImplemented;
			 };
			Get[MainClass.LoftPrefix + "event/{guid}/deny", runAsync: true] = async (dynamic parameters, System.Threading.CancellationToken arg2) =>
			 {
				 return HttpStatusCode.NotImplemented;
			 };
			Get[MainClass.LoftPrefix + "file/get/{fileid}"] = (dynamic arg) =>
			{
				string fileID = arg.fileid;
				string dirPath = "./cache/";
				string filePath = dirPath + fileID;
				string ContentType = "image/jpg";
				var memory = new MemoryStream();
					if (Directory.Exists(dirPath) == false) { Directory.CreateDirectory(dirPath);}
					//verifica se esiste in cache
					if (File.Exists(filePath))
					{
						using (var fileStream = File.OpenRead("./cache/" + fileID))
						{
							fileStream.CopyTo(memory);
						}
					}
					else
					{
						var req = Generic.Drive.Files.Get(fileID);
						var res = req.Execute();

						req.Download(memory);
						memory.Position = 0;
						//salva lo stream in cache
						using (var fileStream = File.Create("./cache/" + fileID))
						{
							memory.CopyTo(fileStream);
						}
					}
					memory.Position = 0;

					return Response.FromStream(memory, ContentType);

			};
			Post[MainClass.LoftPrefix + "event/{id}/update", runAsync:true] = async (dynamic arg1, System.Threading.CancellationToken arg2) =>
			{
				string eventID = arg1.id;
				var inputStr = Request.Body.AsString();
				var i = JsonConvert.DeserializeObject<GoogleEvent>(inputStr);
				if (string.IsNullOrWhiteSpace(eventID) == false)
				{ i.ID = eventID; }
				return await UpdateEvent(i);
			};
		}

		public static string GetEvents()
		{ 
			GoogleEvent.EventsResponse output = new GoogleEvent.EventsResponse();

			EventsResource.ListRequest request = 
				Generic.Calendar.Events.List(MainClass.CalendarId); //primary //loft.luiss@gmail.com //visual1993@gmail.com
			request.TimeMin = DateTime.Now;
			request.ShowDeleted = false;
			request.SingleEvents = true;
			request.MaxResults = 20;
			request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
			// List events.
			Events events = request.Execute();

			EventsResource.ListRequest request1 =
				              Generic.Calendar.Events.List(MainClass.StudentsCalendar); //primary //loft.luiss@gmail.com //visual1993@gmail.com
			request1.TimeMin = DateTime.Now;
			request1.ShowDeleted = false;
			request1.SingleEvents = true;
			request1.MaxResults = 20;
			request1.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
			// List events.
			Events events1 = request1.Execute();

			if (events.Items != null && events.Items.Count > 0)
			{
				foreach (var eventItem in events.Items)
				{
					output.items.Add(eventItem.ToGoogleEvent());
				}
			}

			if (events1.Items != null && events1.Items.Count > 0)
			{
				foreach (var eventItem1 in events1.Items)
				{
					output.items.Add(eventItem1.ToGoogleEvent());
				}
			}
			return JsonConvert.SerializeObject(output);
		}

		public async Task<string> UpdateEvent(GoogleEvent i)
		{
			Event eventoEsistente = null;
			//verifica se fare update o insert
			var IsUpdate = (string.IsNullOrWhiteSpace(i.ID) == false);
			if (i.ID == "null") { IsUpdate = false;}
			if (IsUpdate == false)
			{
				EventsResource.GetRequest request = Generic.Calendar.Events.Get(MainClass.StudentsCalendar, i.ID);
				try { eventoEsistente = await request.ExecuteAsync(); } catch { }
				IsUpdate = eventoEsistente != null;
			}
			GoogleEvent.UpdateResponse output = new GoogleEvent.UpdateResponse();
			try
			{
				eventoEsistente = EventMixedFromGoogle(i,eventoEsistente);
				if (IsUpdate)
				{
					EventsResource.UpdateRequest request = Generic.Calendar.Events.Update(
					eventoEsistente, MainClass.StudentsCalendar, eventoEsistente.Id
				);
					eventoEsistente = await request.ExecuteAsync();
				}
				else {
					eventoEsistente.Id = null;
					EventsResource.InsertRequest request = Generic.Calendar.Events.Insert(
					eventoEsistente, MainClass.StudentsCalendar
				);
					eventoEsistente = await request.ExecuteAsync();
				}
				if (eventoEsistente != null)
				{
					output.item = eventoEsistente.ToGoogleEvent();
					output.state = Visual1993.Data.WebServiceV2.WebRequestState.Ok;
				}
			}
			catch(Exception ex){ 
				output.state = Visual1993.Data.WebServiceV2.WebRequestState.GenericError;
				output.errorMessage = ex.Message;
			}
			//manda la mail al Loft team
			try {
				var msgStr = "<html><body>"+
					"<p>Nome: "+i.Name+"</p>"+
				                 "<p>Inizio: "+i.StartDate.ToString()+"</p>" +
				                 "<p>Fine: " + i.EndDate.ToString() + "</p>" +
				                 "<p>Owner: " + i.OwnerName + "</p>" +
				                 "<p>Luogo: "+i.Luogo+"</p>" +
				                 "<p>Descrizione: "+i.Description+"</p>" +
				                 "<p><a href=\"" + MainClass.ExternalAccessBaseUri + MainClass.LoftPrefix + "event/"+i.InternalEventGuid+"/accept\">Accetta</a></p>" +
				                 "<p><a href=\""+MainClass.ExternalAccessBaseUri+MainClass.LoftPrefix+"event/"+i.InternalEventGuid+"/deny\">Rifiuta</a></p>" +
					"</body></html>"
					;
				var mail = new MailRESTRequest { 
					To="visual1993@gmail.com",
					IsBodyHtml=true,
					SmtpProvider= MailRESTRequest.SmtpProviderEnum.Luiss,
					Body=msgStr,
					Subject="Prenotazione evento Loft"
				};
				var web = new Visual1993.Data.WebServiceV2();
				var config = new WebServiceV2.UrlToStringConfiguration
				{
					url = "http://rest.visual1993.com/visual1993/mail/send",
					Type = WebServiceV2.UrlToStringConfiguration.RequestType.JsonRaw,
					Verb = WebServiceV2.UrlToStringConfiguration.RequestVerb.POST,
					RawContent = JsonConvert.SerializeObject(mail)
				};
				var res = await web.UrlToString(config);
				var resAsObj = JsonConvert.DeserializeObject<WebServiceV2.DefaultResponse>(res);
				if (resAsObj.state != WebServiceV2.WebRequestState.Ok)
				{ output.errorMessage = "mail not sent. Error: " + resAsObj.errorMessage; }
				//ma non impostare lo stato a !=Ok perchè la richiesta in se è andata a buon fine
			}
			catch (Exception ex)
			{
				return JsonConvert.SerializeObject(output);
			}
			return JsonConvert.SerializeObject(output);
		}
		public Nancy.Response MakeResponseString(WebServiceV2.DefaultResponse i, Nancy.HttpStatusCode code = Nancy.HttpStatusCode.OK)
		{
			var jsonString = JsonConvert.SerializeObject(i);
			var response = (Nancy.Response)jsonString;
			response.ContentType = "application/json";
			response.StatusCode = code;
			return response;
		}
		public class MailRESTRequest
		{
			public string To { get; set; }
			public List<string> Cc { get; set; }
			public string Body { get; set; }
			public string Subject { get; set; }
			public bool IsBodyHtml { get; set; } = false;
			public SmtpProviderEnum SmtpProvider { get; set; } = SmtpProviderEnum.Visual1993;

			public enum SmtpProviderEnum { Luiss = 1, Visual1993 = 0 }
		}
		public Event EventMixedFromGoogle(GoogleEvent i, Event j)
		{
			if (i == null) { return null; }
			if (j == null) { j = new Event { Start = new EventDateTime { }, End = new EventDateTime { } };}

			j.Id = i.ID;
			j.Summary = i.Name;
			j.Description = i.Description;
			j.Location = i.Luogo;
			j.Start.DateTime = i.StartDate;
			j.End.DateTime = i.EndDate;
			return j;
		}
	}
}
