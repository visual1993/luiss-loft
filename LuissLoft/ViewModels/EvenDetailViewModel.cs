using System;
using Visual1993;
using Xamarin.Forms;
using CommonClasses;
using System.Linq;
using System.Threading.Tasks;

namespace LuissLoft
{
	public class EventDetailViewModel: ViewModelBase
	{
		public GoogleEvent ObjEvent;
		public User ObjUser;
		public Event ObjInternalEvent;

		public EventDetailViewModel()
		{
		}
		public async Task DownloadData()
		{
			IsLoadingData = true;
			try
			{
				ObjInternalEvent = (await Event.getAllFromGoogleID(ObjEvent.ID)).items.FirstOrDefault();
				ObjUser = (await User.GetOne(ObjInternalEvent?.data?.RelatedOwnerGuid ?? Guid.NewGuid())).items.FirstOrDefault();
			}
			finally { IsLoadingData = false; }
		}
		public void UpdateVM()
		{
			Title = ObjEvent.Name;
			//Img = ObjEvent.Image?.ImageSource?? "loftLogo";
			Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			Description = ObjEvent.Description;
			StartTime = ObjEvent.StartDate.ToString("g");
			EndTime = (ObjEvent.EndDate - ObjEvent.StartDate).ToString("g");

			if (ObjUser != null)
			{
				Autore = ObjUser.data.Nome+" "+ObjUser.data.Cognome;
			}
		}

		string title="Titolo di prova";
		public string Title{
			get { return title;}
			set { title = value; this.RaisePropertyChanged();}
		}

		string autore = "ND";
		public string Autore
		{
			get { return autore; }
			set { autore = value; this.RaisePropertyChanged(); }
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
