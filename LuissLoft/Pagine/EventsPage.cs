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

			listPoints.SetBinding(ListView.IsRefreshingProperty, new Binding(nameof(EventsPageVM.IsLoadingData)));

			listPoints.ItemTapped += (sender, e) =>
			{
				var item = (EventCellVM)e.Item; if (item == null) { return;}
				listPoints.SelectedItem = null;

				var pageViewModel = new EventDetailViewModel { ObjEvent=item.Obj};
				pageViewModel.UpdateVM();
				Navigation.PushAsync(new EventDetailView(pageViewModel));
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
}

