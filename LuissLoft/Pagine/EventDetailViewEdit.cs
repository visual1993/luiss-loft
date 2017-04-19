using System;
using Visual1993;
using Visual1993.Controls;
using Visual1993.Extensions;
using Xamarin.Forms;

namespace LuissLoft
{
	public class EventDetailViewEdit : ContentPage
	{
		EventDetailEditVM VM;
		public EventDetailViewEdit(EventDetailEditVM vm)
		{
			VM = vm ?? new EventDetailEditVM();
			VM.UIPage = this;
			BindingContext = VM;

			var stackIntero = new StackLayout { 
				Orientation= StackOrientation.Vertical
			};
			var image = new Image { };
			image.Bind(nameof(EventDetailEditVM.Img));

			var grigliaData = new Grid { 
				RowDefinitions=new RowDefinitionCollection { 
					new RowDefinition{ Height=GridLength.Auto},
				},
				ColumnDefinitions=new ColumnDefinitionCollection { 
					new ColumnDefinition{Width=GridLength.Star},
					new ColumnDefinition{Width=GridLength.Star},
				}
			};

			var inizio = new DateTimePickerWithLabel { LabelText="Inizio", LabelTextColor= Color.Navy };
			inizio.Bind(nameof(EventDetailEditVM.StartDate), nameof(EventDetailEditVM.StartTime));

			var fine = new DateTimePickerWithLabel { LabelText="Fine", LabelTextColor= Color.Navy, HorizontalOptions= LayoutOptions.End, };
			fine.Bind(nameof(EventDetailEditVM.EndDate), nameof(EventDetailEditVM.EndTime));

			grigliaData.AddChild(inizio, 0,0);
			grigliaData.AddChild(fine, 0, 1);

			var labelTitolo = new Entry { 
				TextColor=Color.Black,
				FontSize= 30
			};
			labelTitolo.Bind(nameof(EventDetailEditVM.Title));

			var labelDescrizione = new Editor
			{
				TextColor = Color.Black,
				FontSize = 25
			};
			labelDescrizione.Bind(nameof(EventDetailEditVM.Description));

			var buttSalva = new Button { Text = "Salva" };
			buttSalva.Clicked+= async delegate {
				VM.UpdateModel();
				var res = await VM.UploadData();
				if (res.state != Visual1993.Data.WebServiceV2.WebRequestState.Ok && res.state != Visual1993.Data.WebServiceV2.WebRequestState.DuplicateExistsOnServer)
				{
					await DisplayAlert("Errore nel salvataggio", res.errorMessage, "Ok");
				}
			};

			stackIntero.Children.Add(image);
			stackIntero.Children.Add(grigliaData);
			stackIntero.Children.Add(labelTitolo);
			stackIntero.Children.Add(labelDescrizione);
			stackIntero.Children.Add(buttSalva);

			var scroller = new ScrollView { Content = stackIntero };
			Content = scroller;
		}
	}
}

