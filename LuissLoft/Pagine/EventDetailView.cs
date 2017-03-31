using System;
using Visual1993;
using Visual1993.Controls;
using Visual1993.Extensions;
using Xamarin.Forms;

namespace LuissLoft
{
	public class EventDetailView : ContentPage
	{
		EventDetailViewModel VM;
		public EventDetailView(EventDetailViewModel vm)
		{
			VM = vm ?? new EventDetailViewModel();
			VM.UIPage = this;
			BindingContext = VM;

			var stackIntero = new StackLayout { 
				Orientation= StackOrientation.Vertical
			};
			var image = new Image { };
			image.Bind(nameof(EventDetailViewModel.Img));

			var grigliaData = new Grid { 
				RowDefinitions=new RowDefinitionCollection { 
					new RowDefinition{ Height=GridLength.Auto},
				},
				ColumnDefinitions=new ColumnDefinitionCollection { 
					new ColumnDefinition{Width=GridLength.Star},
					new ColumnDefinition{Width=GridLength.Star},
				}
			};

			var labelDataInizio = new Label { TextColor= Color.Navy };
			labelDataInizio.Bind(nameof(EventDetailViewModel.StartTime));

			var labelDurata = new Label { TextColor= Color.Navy, HorizontalTextAlignment= TextAlignment.End, };
			labelDurata.Bind(nameof(EventDetailViewModel.EndTime));

			grigliaData.AddChild(labelDataInizio, 0,0);
			grigliaData.AddChild(labelDurata, 0, 1);

			var labelTitolo = new Label { 
				TextColor=Color.Black,
				FontSize= 30
			};
			labelTitolo.Bind(nameof(EventDetailViewModel.Title));

			var labelDescrizione = new Label
			{
				TextColor = Color.Black,
				FontSize = 25
			};
			labelDescrizione.Bind(nameof(EventDetailViewModel.Description));

			stackIntero.Children.Add(image);
			stackIntero.Children.Add(grigliaData);
			stackIntero.Children.Add(labelTitolo);
			stackIntero.Children.Add(labelDescrizione);

			var scroller = new ScrollView { Content = stackIntero };
			Content = scroller;
		}
	}
}

