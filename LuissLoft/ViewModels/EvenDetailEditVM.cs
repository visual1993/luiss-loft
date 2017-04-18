using System;
using Visual1993;
using Visual1993.Extensions;
using Xamarin.Forms;
using CommonClasses;
using System.Linq;
using System.Threading.Tasks;

namespace LuissLoft
{
	public class EventDetailEditVM: ViewModelBase
	{
		public GoogleEvent ObjEvent;

		public EventDetailEditVM()
		{
		}
		public void UpdateVM()
		{
			Title = ObjEvent.Name;
			Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			Description = ObjEvent.Description;
			StartTime = ObjEvent.StartDate.TimeOfDay;
			EndTime = ObjEvent.EndDate.TimeOfDay;
			StartDate = ObjEvent.StartDate;
			EndDate = ObjEvent.EndDate;
		}

		public void UpdateModel()
		{
			ObjEvent.Name=Title;
			//Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			ObjEvent.Description=Description;
			ObjEvent.StartDate=StartDate.SetTime(StartTime);
			ObjEvent.EndDate=EndDate.SetTime(EndTime);
		}

		public async Task UploadData()
		{
			
		}

		string description = "";
		public string Description
		{
			get { return description; }
			set { description = value; this.RaisePropertyChanged(); }
		}

		TimeSpan startTime = DateTime.Now.TimeOfDay;
		public TimeSpan StartTime
		{
			get { return startTime; }
			set { startTime = value; this.RaisePropertyChanged(); }
		}

		TimeSpan endTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(30));
		public TimeSpan EndTime
		{
			get { return endTime; }
			set { endTime = value; this.RaisePropertyChanged(); }
		}

		DateTime startDate = DateTime.Now;
		public DateTime StartDate
		{
			get { return startDate; }
			set { startDate = value; this.RaisePropertyChanged(); }
		}

		DateTime endDate = DateTime.Now.AddMinutes(30);
		public DateTime EndDate
		{
			get { return endDate; }
			set { endDate = value; this.RaisePropertyChanged(); }
		}

		ImageSource img = Globals.DefaultThumb;
		public ImageSource Img
		{
			get { return img; }
			set { img = value; this.RaisePropertyChanged(); }
		}
	}
}
