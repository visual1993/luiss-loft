using System;
using Visual1993;
using Xamarin.Forms;

namespace LuissLoft
{
	public class EventDetailViewModel: ViewModelBase
	{
		public Event ObjEvent;

		public EventDetailViewModel()
		{
		}
		public void UpdateVM()
		{
			Title = ObjEvent.data.Name;
			Img = ObjEvent.data.Image?.ImageSource?? "loftLogo";
		}

		string title="Titolo di prova";
		public string Title{
			get { return title;}
			set { title = value; this.RaisePropertyChanged();}
		}

		ImageSource img = "loftLogo";
		public ImageSource Img
		{
			get { return img; }
			set { img = value; this.RaisePropertyChanged(); }
		}
	}
}
