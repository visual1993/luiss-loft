﻿using System;
using Visual1993;
using Visual1993.Controls;
using Visual1993.Extensions;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommonClasses;

namespace LuissLoft
{
	public class EventCellVM : CellViewModelBase
	{
		public Page UIPage;
		public GoogleEvent Obj;

		public EventCellVM(GoogleEvent i)
		{
			Obj = i;
		}

		public void UpdateVM()
		{
			Title = Obj.Name;
			Copertina = Obj.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			Description = Obj.Description;
			DataInizio = Obj.StartDate.ToString("g");
			DataFormatted = new FormattedString
			{
				Spans = {
					new Span { ForegroundColor=Color.Black,
						Text=Obj.StartDate.Day.ToString("F0"),
						FontAttributes= FontAttributes.Bold, FontSize=Tema.fontSizeLarge,
					},
					new Span{Text=Environment.NewLine},
					new Span { ForegroundColor=Color.Red,
						Text=Obj.StartDate.ToString("MMM").ToUpperInvariant(),
						FontAttributes= FontAttributes.None, FontSize=Tema.fontSizeMedium,
					}
				}
			};
		}
		private string dataInizio = "00/00/0000";
		public string DataInizio { get { return dataInizio; } set { dataInizio = value; this.RaisePropertyChanged(); } }

		private FormattedString dataFormatted = "15";
		public FormattedString DataFormatted { get { return dataFormatted; } set { dataFormatted = value; this.RaisePropertyChanged(); } }

		private string luogo = "";
		public string Luogo { get { return luogo; } set { luogo = value; this.RaisePropertyChanged(); } }

		private string prezzo = "00.00€";
		public string Prezzo { get { return prezzo; } set { prezzo = value; this.RaisePropertyChanged(); } }

		private ImageSource copertina;
		public ImageSource Copertina { get { return copertina; } set { copertina = value; this.RaisePropertyChanged(); } }

	}
}

