using System;
using Visual1993;
using Xamarin.Forms;
using CommonClasses;
using System.Linq;

namespace LuissLoft
{
	public class EventDetailViewModel: ViewModelBase
	{
		public GoogleEvent ObjEvent;

		public EventDetailViewModel()
		{
		}
		public void UpdateVM()
		{
			Title = ObjEvent.Name;
			//Img = ObjEvent.Image?.ImageSource?? "loftLogo";
			Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			Description = ObjEvent.Description;
			StartTime = ObjEvent.StartDate.ToString("g");
			EndTime = (ObjEvent.EndDate - ObjEvent.StartDate).ToString("g");
		}

		string title="Titolo di prova";
		public string Title{
			get { return title;}
			set { title = value; this.RaisePropertyChanged();}
		}

		string description = "Descrizione di prova";
		public string Description
		{
			get { return description; }
			set { description = value; this.RaisePropertyChanged(); }
		}

		string startTime = "0000/00/00";
		public string StartTime
		{
			get { return startTime; }
			set { startTime = value; this.RaisePropertyChanged(); }
		}

		string endTime = "0000/00/00";
		public string EndTime
		{
			get { return endTime; }
			set { endTime = value; this.RaisePropertyChanged(); }
		}

		ImageSource img = Globals.DefaultThumb;
		public ImageSource Img
		{
			get { return img; }
			set { img = value; this.RaisePropertyChanged(); }
		}
	}
}
