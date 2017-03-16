using System;
using Visual1993;
using Visual1993.Controls;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visual1993.Data;

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
			var res = await Event.getAll();
			if (res.IsValidForAtLeastOneItem)
			{
				EventsObj = res.items;
			}
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
		public List<Event> EventsObj = new List<Event>();
		public ObservableCollection<EventCellVM> Events { get; } = new ObservableCollection<EventCellVM>();

		private bool isShowingSerateEliminate = false;
		public bool IsShowingSerateEliminate
		{
			get { return isShowingSerateEliminate;}
			set { isShowingSerateEliminate = value; this.RaisePropertyChanged();}
		}

	}

}

