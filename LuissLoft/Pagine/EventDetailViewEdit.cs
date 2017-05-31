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
					new RowDefinition{ Height=GridLength.Auto},
				},
				ColumnDefinitions=new ColumnDefinitionCollection { 
					new ColumnDefinition{Width=GridLength.Star},
				}
			};

			var inizio = new DateTimePickerWithLabel { LabelText="Inizio", LabelTextColor= Color.Navy };
			inizio.Bind(nameof(EventDetailEditVM.StartDate), nameof(EventDetailEditVM.StartTime));

			var fine = new DateTimePickerWithLabel { LabelText="Fine", LabelTextColor= Color.Navy,};
			fine.Bind(nameof(EventDetailEditVM.EndDate), nameof(EventDetailEditVM.EndTime));

			grigliaData.AddChild(inizio, 0,0);
			grigliaData.AddChild(fine, 1, 0);

			var labelStato = new LabelWithLabelV2 { 
				LabelText="Stato"
			};
			labelStato.Bind(nameof(EventDetailEditVM.Stato));

			var labelAutore = new LabelWithLabelV2
			{
				LabelText = "Richiedente"
			};
			labelAutore.Bind(nameof(EventDetailEditVM.Autore));

			var labelTitolo = new EntryWithLabelV2 { LabelText="Titolo"};
			var le = labelTitolo.Element;
			le.TextColor = Color.Black;
			le.FontSize = 30; 
			le.Placeholder = "Titolo";
			labelTitolo.Bind(nameof(EventDetailEditVM.Title));

			var labelDescrizione = new EditorWithLabelV2
			{
				LabelText="Descrizione",
			};
			labelDescrizione.Bind(nameof(EventDetailEditVM.Description));

			var buttSalva = new Button { Text = "Salva" };
			buttSalva.Clicked+= async delegate {
				if (VM.UpdateModel())
				{
					var luoghiDisponibili = VM.GetLuoghiDisponibili(VM.ObjAllEvents, VM.ObjEvent);
					if (luoghiDisponibili.Count == 0) { await DisplayAlert("Attento","Non sono disponibili luoghi per questi orari","Riprova"); return;}
					var LuogoNessuno = "Nessuno";
					var resLuogo = await DisplayActionSheet("Scegli il luogo in base a quelli disponibili secondo l'orario impostato", LuogoNessuno, null, luoghiDisponibili.ToArray());
					if (string.IsNullOrWhiteSpace(resLuogo) || resLuogo == LuogoNessuno)
					{ return;}
					VM.ObjEvent.Luogo = resLuogo;
					var res = await VM.UploadData();
					if (res.state != Visual1993.Data.WebServiceV2.WebRequestState.Ok && res.state != Visual1993.Data.WebServiceV2.WebRequestState.DuplicateExistsOnServer)
					{
						await DisplayAlert("Errore nel salvataggio", res.errorMessage, "Ok");
					}
					else {
						if (VM.CalendarioVM != null)
						{
							VM.CalendarioVM.DownloadData().ContinueWith(delegate
							{
								VM.CalendarioVM.UpdateVM();
							});
						}
						await DisplayAlert("Evento salvato","L'evento è in attesa di conferma da parte dello staff del Loft","Ok");
						try { await Navigation.PopAsync(); } catch { }
					}
				}
			};

			stackIntero.Children.Add(image);
			stackIntero.Children.Add(labelTitolo);
			stackIntero.Children.Add(grigliaData);
			stackIntero.Children.Add(labelStato);
			stackIntero.Children.Add(labelDescrizione);
			stackIntero.Children.Add(buttSalva);

			var scroller = new ScrollView { Content = stackIntero };
			Content = scroller;
		}
	}
}

