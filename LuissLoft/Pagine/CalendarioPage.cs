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

			calendar.ScheduleCellTapped += (object sender, ScheduleTappedEventArgs args) => {
				var data = args;
			};
			//var listaVacanze = new List<DateTime> { };
			//listaVacanze.Add(DateTime.Now);
			//calendar.BlackoutDates = listaVacanze;

			calendar.SetBinding(SfSchedule.DataSourceProperty, new Binding(nameof(CalendarioPageVM.Items)));
			this.Content = calendar;

		}
	}
}

