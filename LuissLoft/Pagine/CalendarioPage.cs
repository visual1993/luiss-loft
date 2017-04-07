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
using Syncfusion.SfCalendar.XForms;

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

			var calendar = new SfCalendar();

			var listaVacanze = new List<DateTime>();
			listaVacanze.Add(DateTime.Now);
			calendar.BlackoutDates = listaVacanze;
			this.Content = calendar;

		}
	}
}

