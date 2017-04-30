using System;
using Visual1993.Controls;
using Visual1993;

using Xamarin.Forms;

namespace LuissLoft
{
	public class AboutPage : ContentPage
	{
		AboutPageVM VM;
		public AboutPage(AboutPageVM vm)
		{
			VM = vm ?? new AboutPageVM();
			BindingContext = VM;

			var testo = new Label
			{
				Text = "" +
					"Questa app è stata realizzata grazie al lavoro che hanno partecipato al corso di Xamarin tenuto dal Dott. Luca Pisano nell'a.a. 2016-2017"
					+ Environment.NewLine +
								"ciao ciao ciao"
				                                                                                                                                                            
			};
			var scrollVerticale = new ScrollView { 
				Orientation= ScrollOrientation.Vertical,
				Content=testo,
			};

			var stackImmagini = new StackLayout { Orientation= StackOrientation.Vertical };

			var scrollImmagini = new ScrollView
			{
				Content=stackImmagini
			};


			var griglia = new Grid {
				RowDefinitions=new RowDefinitionCollection { 
					new RowDefinition{Height= new GridLength(0.5, GridUnitType.Star)},
					new RowDefinition{Height= new GridLength(0.5, GridUnitType.Star)},
				},
				ColumnDefinitions=new ColumnDefinitionCollection { 
					new ColumnDefinition{Width=GridLength.Star}
				}
			};

			griglia.AddChild(scrollVerticale, 0, 0);
			griglia.AddChild(scrollImmagini, 1, 0);

			Content = griglia;
		}
	}
}

