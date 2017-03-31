using System;
using Visual1993;
using Visual1993.Controls;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visual1993.Data;
using CommonClasses;
using Newtonsoft.Json;

namespace LuissLoft
{
	public class EventsPageVM : ViewModelBase
	{
		public Page UIPage;
		public Guid LocaleGuid;
		public List<GoogleEvent> EventsObj = new List<GoogleEvent>();
		public ObservableCollection<EventCellVM> Events { get; } = new ObservableCollection<EventCellVM>();

		public static Color ColoreSfondoLista = Color.FromHex("#ecf7f7");

		public EventsPageVM()
		{
		}
		public async Task DownloadData()
		{
			IsLoadingData = true;
			/*var res = await Event.getAll();
			if (res.IsValidForAtLeastOneItem)
			{
				EventsObj = res.items;
			}*/
			var res = await DownloadEvents();
			EventsObj = res.items;
			IsLoadingData = false;
		}
		public void UpdateVM()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				Events.Clear();
				foreach (var item in EventsObj)
				{
					var cellVM = new EventCellVM(item);
					cellVM.UpdateVM();
					Events.Add(cellVM);
				}
				if (Events.Count == 0) { IsListEmpty = true; } else { IsListEmpty = false; }
			});
		}


		private bool isShowingSerateEliminate = false;
		public bool IsShowingSerateEliminate
		{
			get { return isShowingSerateEliminate; }
			set { isShowingSerateEliminate = value; this.RaisePropertyChanged(); }
		}
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
}
