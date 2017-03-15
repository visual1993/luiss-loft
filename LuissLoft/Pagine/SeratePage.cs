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

	public class SeratePage : ContentPage
	{
		public EventiPageVM VM;
		public SeratePage(EventiPageVM vm)
		{
			VM = vm; BindingContext = VM;
			var listPoints = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				IsPullToRefreshEnabled = true,
				RowHeight=250, HasUnevenRows=false,
				BackgroundColor=EventiPageVM.ColoreSfondoLista
			};
			listPoints.ItemTemplate = new DataTemplate(typeof(EventCellView));
			listPoints.SetBinding(ListView.ItemsSourceProperty, new Binding(nameof(SeratePageVM.Serate)));
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

	public class EventiPageVM : ViewModelBase
	{
		public Page UIPage;
		public Guid LocaleGuid;

		public static Color ColoreSfondoLista = Color.FromHex("#ecf7f7");

		public EventiPageVM()
		{
		}
		public async Task DownloadData()
		{ }
		public void UpdateVM()
		{
			
		}
		public List<Event> SerateObj = new List<Event>();
		public ObservableCollection<EventCellVM> Serate { get; } = new ObservableCollection<EventCellVM>();

		private bool isShowingSerateEliminate = false;
		public bool IsShowingSerateEliminate
		{
			get { return isShowingSerateEliminate;}
			set { isShowingSerateEliminate = value; this.RaisePropertyChanged();}
		}

	}

}

