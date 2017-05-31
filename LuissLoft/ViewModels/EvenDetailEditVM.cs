using System;
using Visual1993;
using Visual1993.Extensions;
using Xamarin.Forms;
using CommonClasses;
using System.Linq;
using System.Threading.Tasks;
using Visual1993.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LuissLoft
{
	public class EventDetailEditVM : ViewModelBase
	{
		public CalendarioPageVM CalendarioVM;

		public List<GoogleEvent> ObjAllEvents = new List<GoogleEvent>();
		public GoogleEvent ObjEvent;
		public Event ObjInternalEvent;
		public User ObjUser;

		public bool IsNew;

		public EventDetailEditVM()
		{
		}

		public async Task DownloadData(bool overwrite = false)
		{
			if (IsNew)
			{ return; }

			IsLoadingData = true;
			try
			{
				if (overwrite == true || ObjInternalEvent == null)
				{
					ObjInternalEvent = (await Event.getAllFromGoogleID(ObjEvent.ID)).items.FirstOrDefault();
				}
				ObjUser = (await User.GetOne(ObjInternalEvent?.data?.RelatedOwnerGuid ?? Guid.NewGuid())).items.FirstOrDefault();
			}
			finally { IsLoadingData = false; }
		}
		public void UpdateVM()
		{
			if (IsNew)
			{

				return;
			}

			Title = ObjEvent.Name;
			Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			Description = ObjEvent.Description;
			StartTime = ObjEvent.StartDate.TimeOfDay;
			EndTime = ObjEvent.EndDate.TimeOfDay;
			StartDate = ObjEvent.StartDate;
			EndDate = ObjEvent.EndDate;

			if (ObjInternalEvent != null)
			{
				Stato = ObjInternalEvent.data.State.ToString();
			}
			if (IsNew) { ObjUser = App.VM.user; }
			if (ObjUser != null)
			{
				Autore = ObjUser.data.Nome + " " + ObjUser.data.Cognome;
			}

		}

		public bool UpdateModel()
		{

			if (App.VM.user == null) { UIPage.DisplayAlert("Attenzione", "E' necessario loggarsi", "Ok"); return false; }

			if (IsNew)
			{
				ObjEvent = new GoogleEvent { ID = "" };
				ObjInternalEvent = new Event
				{
					Guid = Guid.NewGuid(),
					data = new Event.PersonalizedData
					{
						RelatedOwnerGuid = App.VM.user.Guid,
						State = Event.PersonalizedData.EventStateEnum.Inserted
					}
				};
			}

			ObjEvent.Name = Title;
			//Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			ObjEvent.Description = Description;
			ObjEvent.StartDate = StartDate.SetTime(StartTime);
			ObjEvent.EndDate = EndDate.SetTime(EndTime);
			ObjEvent.InternalEventGuid = ObjInternalEvent.Guid;
			ObjEvent.OwnerName = App.VM.user?.data?.Nome ?? "ND" + " " + App.VM.user?.data?.Cognome ?? "ND" + " - " + App.VM.user?.data?.Email ?? "ND";
			ObjInternalEvent.data.State = Event.PersonalizedData.EventStateEnum.Pending;

			if (
				ObjEvent.EndDate < ObjEvent.StartDate
				||
				ObjEvent.StartDate < DateTime.Now
			)
			{
				UIPage.DisplayAlert("Attenzione", "Errore nella data", "Ok"); return false;
			}
			var dataInizio_ = ObjEvent.StartDate.TimeOfDay; var dataFine = ObjEvent.EndDate.TimeOfDay;
			if (
				(dataInizio_.Hours >= 8 && (dataInizio_.Hours < 19 && dataInizio_.Minutes <= 30))
				&&
				(dataFine.Hours >= 8 && (dataFine.Hours < 20))
			   )
			{ }
			else
			{
				UIPage.DisplayAlert("Attenzione", "Il LOFT è chiuso", "Ok"); return false;
			}
			if ((ObjEvent.EndDate - ObjEvent.StartDate) < Globals.DurataMinimaEvento)
			{
				UIPage.DisplayAlert("Attenzione", "Prenotazione minima " + Globals.DurataMinimaEvento.Minutes + " minuti", "Ok"); return false;
			}
			return true;
		}
		public class GoogleEventUpdateRequest
		{
			public Guid RelatedOwnerGuid { get; set; }
			public GoogleEvent ObjGoogleEvent { get; set; }
		}
		public async Task<GoogleEvent.UpdateResponse> UploadData()
		{
			IsLoadingData = true;
			ObjEvent.FixDates(false);

			var request = new GoogleEventUpdateRequest { 
				ObjGoogleEvent=ObjEvent,
				RelatedOwnerGuid=App.VM.user.Guid,
			};
			try
			{
				var ws = new WebServiceV2();
				var config = new WebServiceV2.UrlToStringConfiguration
				{
					url = Globals.RestApiV1 + "loft/event/" + ObjEvent.ID + "/update",
					Type = WebServiceV2.UrlToStringConfiguration.RequestType.JsonRaw,
					Verb = WebServiceV2.UrlToStringConfiguration.RequestVerb.POST,
					RawContent = JsonConvert.SerializeObject(request),
					timeout= TimeSpan.FromSeconds(60)
				};
				if (string.IsNullOrWhiteSpace(ObjEvent.ID)) {
					config.url = Globals.RestApiV1 + "loft/event/null/update";
				}
				var res = await ws.UrlToString(config);
				if (res == null) { Visual1993.Console.WriteLine("bad response"); return new GoogleEvent.UpdateResponse();}
				var resObj= JsonConvert.DeserializeObject<GoogleEvent.UpdateResponse>(res);
				if (resObj != null) {
					this.ObjEvent = resObj.item;
					if (this.ObjEvent == null) {
						return resObj;
					}
					this.ObjInternalEvent.data.RelatedGoogleEventID = this.ObjEvent.ID;
					if (IsNew == false) { 
						var resInterno = await this.ObjInternalEvent.update();
					}
					if (IsNew)
					{
						var resInterno = await this.ObjInternalEvent.insert();
						IsNew = false;
					}//qui non ci posso mettere l'else
				}
				return resObj;
			}
			finally {
				IsLoadingData = false;
			}
		}

		public List<string> GetLuoghiDisponibili(List<GoogleEvent> eventi, GoogleEvent eventoInCorsoDiModifica=null)
		{
			var listaLuoghi = new List<LuoghiEnum>();
			foreach (LuoghiEnum luogoEnum in Enum.GetValues(typeof(LuoghiEnum)))
			{
				listaLuoghi.Add(luogoEnum);
			}
			var o = new List<string>();
			if (eventi == null) { }
			//StartDate ed EndDate sono già valorizzati con SetTime perchè dopo .UpdateModel()
			var luoghiDisponibiliTot = listaLuoghi.ToList();
			foreach (var evento in GetOnlyEventsFromWithinDateTime(eventi, ObjEvent.StartDate, ObjEvent.EndDate))
			{
				if (eventoInCorsoDiModifica != null && evento.ID == eventoInCorsoDiModifica?.ID) { continue; }
				System.Console.WriteLine("luogo occupato: "+evento.Luogo);
				var luoghiOccupatiPerSingoloEvento = new List<LuoghiEnum>();
				foreach (var luogoString in Globals.Luoghi)
				{
					if (string.IsNullOrWhiteSpace(evento.Luogo)) { continue; } //cioè non aggiungerlo a quelli occupati
					if (
						luogoString.LuogoStringa.ToLowerInvariant().Contains(evento.Luogo.ToLowerInvariant())
						||
						evento.Luogo.ToLowerInvariant().Contains(luogoString.LuogoStringa.ToLowerInvariant())
					  )
					{
						//se è presente tuttoLoft, ritorna
						if (luogoString.LuogoEnum == LuoghiEnum.Intero) { return new List<string>(); }
						try { luoghiOccupatiPerSingoloEvento.Add(luogoString.LuogoEnum); } catch { }
					}
				}
				if (luoghiOccupatiPerSingoloEvento.Count > 0)
				{
					//allora elimina il tutto loft
					try { luoghiDisponibiliTot.Remove(LuoghiEnum.Intero); } catch { }
				}
				foreach (var luogoOccupatoTemp in luoghiOccupatiPerSingoloEvento.ToList())
				{
					try { luoghiDisponibiliTot.Remove(luogoOccupatoTemp); } catch { }
				}
			}
			foreach (var luogoDisponibile in luoghiDisponibiliTot)
			{
				var luogoDisponibileStr = "";
				luogoDisponibileStr = Globals.Luoghi.FirstOrDefault(x=>x.IsCorretto==true && x.LuogoEnum==luogoDisponibile)?.LuogoStringa??"";

				if (string.IsNullOrWhiteSpace(luogoDisponibileStr) == false)
				{
					o.Add(luogoDisponibileStr);
				}
			}
			return o;
		}
		public List<GoogleEvent> GetOnlyEventsFromWithinDateTime(IEnumerable<GoogleEvent> i, DateTime inizio, DateTime fine)
		{
			var o = new List<GoogleEvent>();
			//se l'evento è all'interno della fascia oraria
			var eventi = i.Where(x =>
				(x.StartDate >= inizio && x.EndDate <= fine)
				||
				(x.StartDate <= inizio && x.EndDate >= fine)
			);
			o = eventi.ToList();
			return o;
		}
		string description = "";
		public string Description
		{
			get { return description; }
			set { description = value; this.RaisePropertyChanged(); }
		}

		string stato = Event.PersonalizedData.EventStateEnum.Pending.ToString();
		public string Stato
		{
			get { return stato; }
			set { stato = value; this.RaisePropertyChanged(); }
		}

		string autore = "ND";
		public string Autore
		{
			get { return autore; }
			set { autore = value; this.RaisePropertyChanged(); }
		}


		TimeSpan startTime = DateTime.Now.TimeOfDay;
		public TimeSpan StartTime
		{
			get { return startTime; }
			set { startTime = value; this.RaisePropertyChanged(); }
		}

		TimeSpan endTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(30));
		public TimeSpan EndTime
		{
			get { return endTime; }
			set { endTime = value; this.RaisePropertyChanged(); }
		}

		DateTime startDate = DateTime.Now;
		public DateTime StartDate
		{
			get { return startDate; }
			set { startDate = value; this.RaisePropertyChanged(); }
		}

		DateTime endDate = DateTime.Now.AddMinutes(30);
		public DateTime EndDate
		{
			get { return endDate; }
			set { endDate = value; this.RaisePropertyChanged(); }
		}

		ImageSource img = Globals.DefaultThumb;
		public ImageSource Img
		{
			get { return img; }
			set { img = value; this.RaisePropertyChanged(); }
		}
	}
}
