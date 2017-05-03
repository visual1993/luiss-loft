using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using Newtonsoft.Json;
using Visual1993.Data;
using Visual1993;
using System.Threading.Tasks;

using Xamarin.Forms;
using Syncfusion.SfSchedule.XForms;

namespace LuissLoft
{
	public class CalendarioPage : ContentPage
	{
		public CalendarioPageVM VM;
		public CalendarioPage(CalendarioPageVM vm)
		{
			VM = vm ?? new CalendarioPageVM();
			VM.UIPage = this;
			BindingContext = VM;

			var calendar = new SfSchedule {
				ShowAppointmentsInline=true,
				ScheduleView= ScheduleView.WeekView
			};
			ScheduleAppointmentMapping dataMapping = new ScheduleAppointmentMapping();
			dataMapping.SubjectMapping = nameof(GoogleEventVM.Title);
			dataMapping.StartTimeMapping = nameof(GoogleEventVM.StartDate);
			dataMapping.EndTimeMapping = nameof(GoogleEventVM.EndDate);
			//dataMapping.ColorMapping = "color";
			calendar.AppointmentMapping = dataMapping;

			calendar.ScheduleCellTapped += async(object sender, ScheduleTappedEventArgs args) =>
			{
				var data = args;
				var fine = args.datetime.Add(TimeSpan.FromMinutes(calendar.TimeInterval));
				var events = VM.GetEventsFromDateTime(args.datetime, TimeSpan.FromMinutes(calendar.TimeInterval));
				if (events != null && events.Count > 0)
				{
					var IsNuovoEvento = false;
					var stringaCrea = "Crea evento";
					var eventiListaStringhe = events.Select(x => x.Name).ToList();
					eventiListaStringhe.Add(stringaCrea);

					var stringaScelta = await DisplayActionSheet("Quale evento?", "Nessuno", null, eventiListaStringhe.ToArray());
					if (stringaScelta == stringaCrea) {
						IsNuovoEvento = true;
						await DoCrea(args.datetime, fine);
						return;
					}


					var eventoScelto = VM.EventsObj.FirstOrDefault(x => x.Name == stringaScelta);
					if (eventoScelto != null)
					{
						if (App.VM.user != null)
						{
							//se è modificabile, aprilo in edit
							var eventoInterno = VM.EventiInterniObj.FirstOrDefault(x => x.data.RelatedGoogleEventID == eventoScelto.ID);
							if (IsNuovoEvento||(eventoInterno != null && eventoInterno.data.RelatedOwnerGuid == App.VM.user.Guid))
							{
								var pageEditVM = new EventDetailEditVM {
									CalendarioVM=VM, ObjEvent = eventoScelto, ObjInternalEvent = eventoInterno,
									ObjAllEvents = VM.EventsObj
								};
								pageEditVM.DownloadData(false).ContinueWith(delegate {
									pageEditVM.UpdateVM();
								}); 
								await Navigation.PushAsync(new EventDetailViewEdit(pageEditVM));
							}
							else
							{  //altrimenti, aprilo in read only
								var pageViewVM = new EventDetailViewModel { ObjEvent = eventoScelto };
								pageViewVM.DownloadData().ContinueWith(delegate
								{
									pageViewVM.UpdateVM();
								});
								await Navigation.PushAsync(new EventDetailView(pageViewVM));
							}
						}
						else
						{  //altrimenti, aprilo in read only
							var pageViewVM = new EventDetailViewModel { ObjEvent = eventoScelto };
							pageViewVM.UpdateVM(); //non c'è bisogno qui di fare il download data perchè già ho gli oggetti interi che sto passando
							await Navigation.PushAsync(new EventDetailView(pageViewVM));
						}
					}
					else
					{
						//se non ho scelto nessun evento torna indietro
						return;
					}
				}
				else if(events == null || events.Count == 0) {
					//se non ci sono già eventi in questa fascia oraria, permettine la creazione
					await DoCrea(args.datetime, fine);
				}
			};
			//var listaVacanze = new List<DateTime> { };
			//listaVacanze.Add(DateTime.Now);
			//calendar.BlackoutDates = listaVacanze;

			calendar.SetBinding(SfSchedule.DataSourceProperty, new Binding(nameof(CalendarioPageVM.Items)));
			this.Content = calendar;

		}
		public async Task DoCrea(DateTime start, DateTime fine)
		{
			var pageEditVM = new EventDetailEditVM
			{
				IsNew = true,
				CalendarioVM = VM,
				StartDate = start,
				StartTime = start.TimeOfDay,
				EndDate = fine,
				EndTime = fine.TimeOfDay,
				ObjAllEvents = VM.EventsObj
			};
			pageEditVM.UpdateVM();
			await Navigation.PushAsync(new EventDetailViewEdit(pageEditVM));
		}
	}

}

