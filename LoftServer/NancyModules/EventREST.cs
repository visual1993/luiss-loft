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
			Get[MainClass.LoftPrefix + "event/{guid:guid}", runAsync: true] = async (dynamic parameters, System.Threading.CancellationToken arg2) =>
			 {
				var eventoGuid = (Guid)parameters.guid;
				 var evento = (await InternalEvent.GetOne(eventoGuid)).items.FirstOrDefault();
				 if (evento == null) { return HttpStatusCode.NotFound; }
				 return JsonConvert.SerializeObject(evento);
			 };
			Get[MainClass.LoftPrefix + "event/{guid:guid}/accept", runAsync: true] = async (dynamic parameters, System.Threading.CancellationToken arg2) =>
			 {
				 var eventoGuid = (Guid)parameters.guid;
				var evento = (await InternalEvent.GetOne(eventoGuid)).items.FirstOrDefault();
				if (evento == null) { return HttpStatusCode.NotFound; }
				 evento.data.State = InternalEvent.PersonalizedData.EventStateEnum.Approved;
				 var resUpdate = await evento.update();

				Event googleEvent = null; User utente = null;
				 try
				 {
					 googleEvent = await Generic.Calendar.Events.Get(MainClass.StudentsCalendar, evento.data.RelatedGoogleEventID).ExecuteAsync();
					 utente = (await User.GetOne(evento.data.RelatedOwnerGuid)).items.FirstOrDefault();
				 }
				 catch { }

				 //TODO:manda la mail all'utente
				 if (utente != null && googleEvent != null)
				 {
					 await SendMail("Prenotazione evento LOFT", "La tua prenotazione per "
									+ (googleEvent?.End?.DateTime ?? DateTime.MinValue).ToString("f")
									+ "è stata ACCETTATA. " +
								   "Rispondi a questa email per chiedere informazioni",
									MainClass.StaffMail, utente.data.Email
								   );
				 }

				 if (resUpdate.state == WebServiceV2.WebRequestState.Ok)
				 { return "Evento ACCETTATO"; }
				 else { return resUpdate.errorMessage;}
			 };
			Get[MainClass.LoftPrefix + "event/{guid:guid}/deny", runAsync: true] = async (dynamic parameters, System.Threading.CancellationToken arg2) =>
			 {
				 var eventoGuid = (Guid)parameters.guid;
				 var evento = (await InternalEvent.GetOne(eventoGuid)).items.FirstOrDefault();
				 if (evento == null) { return HttpStatusCode.NotFound; }
				evento.data.State = InternalEvent.PersonalizedData.EventStateEnum.Rejected;
				 var resUpdate = await evento.update();
				 //ora cancellalo anche da Google Calendar
				Event googleEvent = null; User utente = null;
				 try
				 {
					 googleEvent = await Generic.Calendar.Events.Get(MainClass.StudentsCalendar, evento.data.RelatedGoogleEventID).ExecuteAsync();
					 utente = (await User.GetOne(evento.data.RelatedOwnerGuid)).items.FirstOrDefault();
				 }
				 catch { }
				 var request = Generic.Calendar.Events.Delete(
					 MainClass.StudentsCalendar,
					evento.data.RelatedGoogleEventID
				 );
				 await request.ExecuteAsync();

				 //TODO:manda la mail all'utente
				 if (utente != null && googleEvent != null)
				 {
					 await SendMail("Prenotazione evento LOFT", "La tua prenotazione per il "
									+ (googleEvent?.End?.DateTime ?? DateTime.MinValue).ToString("f")
									+ "è stata RIFIUTATA." +
								   "Rispondi a questa email per chiedere informazioni",
									MainClass.StaffMail, utente.data.Email
								   );
				 }
				 if (resUpdate.state == WebServiceV2.WebRequestState.Ok)
				 { return "Evento RIFIUTATO ed eliminato da Google Calendar"; }
				 else { return resUpdate.errorMessage; }
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
				var h = JsonConvert.DeserializeObject<GoogleEventUpdateRequest>(inputStr);
				var i = h.ObjGoogleEvent;
				if (string.IsNullOrWhiteSpace(eventID) == false)
				{ i.ID = eventID; }
				return await UpdateEvent(i, h.RelatedOwnerGuid);
			};
		}
		public class GoogleEventUpdateRequest
		{
			public Guid RelatedOwnerGuid { get; set; }
			public GoogleEvent ObjGoogleEvent { get; set; }
		}
		public static string GetEvents()
		{ 
			GoogleEvent.EventsResponse output = new GoogleEvent.EventsResponse();

			EventsResource.ListRequest request = 
				Generic.Calendar.Events.List(MainClass.CalendarId); //primary //loft.luiss@gmail.com //visual1993@gmail.com
			request.TimeMin = DateTime.Now;
			request.ShowDeleted = false;
			request.SingleEvents = true;
			request.MaxResults = 50;
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

		public async Task<string> UpdateEvent(GoogleEvent i, Guid OwnerGuid)
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
			//prendi le info dell'utente per la mail
			//var internalEvent = (await InternalEvent.getAllFromGoogleID(i.ID)).items.FirstOrDefault();
			//if (internalEvent == null) { output.errorMessage = "Impossibile trovare l'evento correlato"; return JsonConvert.SerializeObject(output);}
			var utente = (await User.GetOne(OwnerGuid)).items.FirstOrDefault();
			if (utente == null) { output.errorMessage = "Impossibile trovare l'utente che ha inserito l'evento"; return JsonConvert.SerializeObject(output); }
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
					From=utente?.data?.Email??null,
					To=MainClass.StaffMail,
					Cc=MainClass.StaffCcMail,
					IsBodyHtml=true,
					SmtpProvider= MailRESTRequest.SmtpProviderEnum.Luiss,
					Body=msgStr,
					Subject="Prenotazione evento Loft"
				};
				var web = new Visual1993.Data.WebServiceV2();
				var config = new WebServiceV2.UrlToStringConfiguration
				{
					url=MainClass.Visual1993RestServer+"visual1993/mail/send",
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
				output.errorMessage = ex.Message;
			}
			return JsonConvert.SerializeObject(output);
		}
		public async Task<WebServiceV2.DefaultResponse> SendMail(string oggetto, string msgStr, string from, string to, bool isHtml = false)
		{
			var output = new WebServiceV2.DefaultResponse();
			try
			{
				var mail = new MailRESTRequest
				{
					From = from,
					To = to,
					IsBodyHtml = isHtml,
					SmtpProvider = MailRESTRequest.SmtpProviderEnum.Luiss,
					Body = msgStr,
					Subject = oggetto
				};
				var web = new Visual1993.Data.WebServiceV2();
				var config = new WebServiceV2.UrlToStringConfiguration
				{
					url = MainClass.Visual1993RestServer+"visual1993/mail/send",
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
				output.errorMessage = ex.Message;
			}
			return output;
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
			public string From { get; set; }
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
			if (j == null) { j = new Event { Start = new EventDateTime { TimeZone="Europe/Rome" }, End = new EventDateTime { TimeZone = "Europe/Rome" } };}

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
