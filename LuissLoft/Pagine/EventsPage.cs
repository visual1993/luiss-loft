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

	public class EventsPage : ContentPage
	{
		public EventsPageVM VM;
		public EventsPage(EventsPageVM vm)
		{
			VM = vm; BindingContext = VM;
			var listPoints = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				IsPullToRefreshEnabled = true,
				//RowHeight=250,
                HasUnevenRows =true,
				BackgroundColor=EventsPageVM.ColoreSfondoLista
			};
			listPoints.ItemTemplate = new DataTemplate(typeof(EventCellView));
			listPoints.SetBinding(ListView.ItemsSourceProperty, new Binding(nameof(EventsPageVM.Events)));
			listPoints.ItemTapped += (sender, e) =>
			{
				var item = (EventCellVM)e.Item; if (item == null) { return;}
				listPoints.SelectedItem = null;

				var pageViewModel = new EventDetailViewModel { ObjEvent=item.Obj};
				pageViewModel.UpdateVM();
				Navigation.PushAsync(new EventDetailXAMLView(pageViewModel));
			};
			listPoints.Refreshing += async (sender, e) =>
			{
				await VM.DownloadData().ContinueWith(delegate {
					VM.UpdateVM();
				});
				listPoints.IsRefreshing = false;
			};

			var loading = new ActivityIndicator { IsRunning = false, IsVisible = false };
			loading.Bind(nameof(ViewModelBase.IsLoadingData));
			loading.BindingContext = VM;

			var labelListaVuota = new Label { Text = "Non ci sono ancora elementi qui", FontAttributes = FontAttributes.Italic, HorizontalOptions = LayoutOptions.Center };
			labelListaVuota.SetBinding(View.IsVisibleProperty, new Binding(nameof(ViewModelBase.IsListEmpty)));
			Title = "Eventi";

			Content = new StackLayout
			{
				Children = {
					loading,
					labelListaVuota,
					listPoints,
				}
			};

		}
	}

	public class EventsPageVM : ViewModelBase
	{
		public Page UIPage;
		public Guid LocaleGuid;

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
			Device.BeginInvokeOnMainThread(() => {
				Events.Clear();
				foreach (var item in EventsObj)
				{
					var cellVM = new EventCellVM(item);
					cellVM.UpdateVM();
					Events.Add(cellVM);
				}
				if (Events.Count == 0) { IsListEmpty = true; }
			});
		}
		public List<GoogleEvent> EventsObj = new List<GoogleEvent>();
		public ObservableCollection<EventCellVM> Events { get; } = new ObservableCollection<EventCellVM>();

		private bool isShowingSerateEliminate = false;
		public bool IsShowingSerateEliminate
		{
			get { return isShowingSerateEliminate;}
			set { isShowingSerateEliminate = value; this.RaisePropertyChanged();}
		}
		public async Task<CommonClasses.GoogleEvent.EventsResponse> DownloadEvents()
		{
			try
			{
				var ws = new Visual1993.Data.WebServiceV2();
				var res = await ws.UrlToString(Globals.RestApiV1+"loft/events");
				return JsonConvert.DeserializeObject<GoogleEvent.EventsResponse>(res);
			}
			catch {
				return new GoogleEvent.EventsResponse();
			}
		}
	}

}

