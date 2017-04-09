using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CommonClasses;
using Xamarin.Forms;

using Syncfusion.SfSchedule.XForms;

namespace LuissLoft
{
	public class CalendarioPageVM : ViewModelBase
	{
		public event EventHandler<User> Completed;
		public virtual void OnCompleted(User user)
		{
			EventHandler<User> handler = Completed;
			if (handler != null)
			{
				handler(this, user);
			}
		}
		public CalendarioPageVM()
		{
		}

		public async Task DownloadData()
		{
			IsLoadingData = true;
			try
			{
				EventsObj = (await DownloadEvents()).items;
			}
			finally
			{
				IsLoadingData = false;
			}
		}
		public void UpdateVM() {
			Device.BeginInvokeOnMainThread(() =>
			{
				Items.Clear();
				foreach (var evento in EventsObj)
				{
					var cellVM = new GoogleEventVM(evento);
					cellVM.UpdateVM();
					Items.Add(cellVM);
				}
			});
		}

		/*private string password = "";
		public string Password { get { return password; } set { password = value; this.RaisePropertyChanged(); } }
		*/
		public List<GoogleEvent> EventsObj = new List<GoogleEvent>();
		public ObservableCollection<GoogleEventVM> Items { get; } = new ObservableCollection<GoogleEventVM>();
		public async Task<CommonClasses.GoogleEvent.EventsResponse> DownloadEvents()
		{
			try
			{
				var ws = new Visual1993.Data.WebServiceV2();
				var res = await ws.UrlToString(Globals.RestApiV1 + "loft/events");
				return JsonConvert.DeserializeObject<GoogleEvent.EventsResponse>(res);
			}
			catch
			{
				return new GoogleEvent.EventsResponse();
			}
		}

	}
	public class GoogleEventVM : CellViewModelBase
	{
		public GoogleEventVM(GoogleEvent i)
		{
			ObjEvent = i;
		}
		public void UpdateVM()
		{
			Title = ObjEvent.Name;
			Description = ObjEvent.Description;
			Place = ObjEvent.Luogo;
			StartDate = ObjEvent.StartDate;
			EndDate = ObjEvent.EndDate;
		}
		public GoogleEvent ObjEvent;

		DateTime startDate { get; set; }
		public DateTime StartDate
		{
			get { return startDate; }
			set { startDate = value; this.RaisePropertyChanged(); }
		}

		DateTime endDate { get; set; }
		public DateTime EndDate
		{
			get { return endDate; }
			set { endDate = value; this.RaisePropertyChanged(); }
		}

		string place { get; set; }
		public string Place
		{
			get { return place; }
			set { place = value; this.RaisePropertyChanged(); }
		}
	}
}
