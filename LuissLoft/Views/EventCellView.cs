using System;
using Visual1993;
using Visual1993.Controls;
using Visual1993.Extensions;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Visual1993.Data;

namespace LuissLoft
{
	public class EventCellView : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var VM = BindingContext as EventCellVM;
			if (VM == null)
				return;
			ContextActions.Clear();
			if (VM.IsEditable)
			{
				var menuRemove = new MenuItem
				{
					Text = "Rimuovi",
					IsDestructive = true
				};
				menuRemove.Clicked += (sender, e) =>
				{
					var item = (EventCellVM)BindingContext; if (item == null) { return; }
				};
			}
		}

		public EventCellView()
		{
			var layoutIntero = new Grid
			{
				HeightRequest = 300,
				BackgroundColor = Color.Transparent,
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition { Height=new GridLength(0.70, GridUnitType.Star) },
					new RowDefinition { Height=new GridLength(0.30, GridUnitType.Star) }
				},
				RowSpacing = 0,
				Padding = new Thickness(5),
			};
			var grigliaTesto = new Grid
			{
				BackgroundColor = Color.White,
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition { Height=GridLength.Auto },
					new RowDefinition { Height=GridLength.Auto }
				},
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition { Width=50 },
					new ColumnDefinition { Width=GridLength.Star }
				},
				Padding = new Thickness(5, 0, 5, 0)
			};
			var title = new Label
			{
				TextColor = Color.Black,
				FontSize = DefaultTema.FontMedium,
				FontAttributes = FontAttributes.Bold,
				HorizontalTextAlignment = TextAlignment.Start,
				HorizontalOptions = LayoutOptions.Start,
			};
			title.SetBinding(Label.TextProperty, nameof(EventCellVM.Title));

			var data = new Label { TextColor = Color.White, FontSize = DefaultTema.FontSmall };
			data.SetBinding(Label.TextProperty, nameof(EventCellVM.DataInizio));

			var luogoLabel = new Label { TextColor = Color.White };
			luogoLabel.SetBinding(Label.TextProperty, nameof(EventCellVM.Luogo));

			var prezzoLabel = new Label { TextColor = Color.White };
			prezzoLabel.SetBinding(Label.TextProperty, nameof(EventCellVM.Prezzo));

			var startLabel = new Label { };
			startLabel.SetBinding(Label.FormattedTextProperty, nameof(EventCellVM.DataFormatted));

#if __DROID__
			var img = new FFImageLoading.Forms.CachedImage()
			{
				Aspect = Aspect.AspectFill,
				LoadingPriority = FFImageLoading.Work.LoadingPriority.Low,
				CacheDuration = TimeSpan.FromDays(360),
				ErrorPlaceholder = Globals.DefaultThumb,
				LoadingPlaceholder = Globals.DefaultThumb,
			};
			img.SetBinding(SpeedImage.SourceProperty, nameof(EventCellVM.Copertina));
#else
			var img = new Image()
			{
				Aspect = Aspect.AspectFill,
			};
			img.SetBinding(Image.SourceProperty, nameof(EventCellVM.Copertina));
			#endif

			//var img = new Image() { Aspect = Aspect.AspectFill,};
			//img.Bind(nameof(EventCellVM.Copertina));

			var layoutDataLuogo = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Spacing = 5,
				Children = { new Label { Text = "@", TextColor = Color.Pink }, luogoLabel, data }
			};
			var boxView = new BoxView { Color = Color.Black, Opacity = 0.5 }; //prima era 0.8

			grigliaTesto.AddChild(startLabel, 0, 0, 2, 1);
			grigliaTesto.AddChild(title,0,1,1,1);

			layoutIntero.AddChild(img, 0,0,1,1);
			layoutIntero.AddChild(grigliaTesto,1,0,1,1);

			layoutIntero.Margin = new Thickness(5);

			View = layoutIntero;
		}

	}
}
