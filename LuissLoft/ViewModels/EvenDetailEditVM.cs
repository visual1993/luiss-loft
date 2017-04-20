using System;
using Visual1993;
using Visual1993.Extensions;
using Xamarin.Forms;
using CommonClasses;
using System.Linq;
using System.Threading.Tasks;
using Visual1993.Data;
using Newtonsoft.Json;

namespace LuissLoft
{
	public class EventDetailEditVM: ViewModelBase
	{
		public GoogleEvent ObjEvent;
		public Event ObjInternalEvent;
		public bool IsNew;

		public EventDetailEditVM()
		{
		}

		public async Task DownloadData()
		{
			if (IsNew)
			{return;}

			IsLoadingData = true;
			try
			{
				ObjInternalEvent = (await Event.getAllFromGoogleID(ObjEvent.ID)).items.FirstOrDefault();
			}
			finally { IsLoadingData = false; }
		}
		public void UpdateVM()
		{
			if (IsNew) { 
				
				return;
			}

			Title = ObjEvent.Name;
			Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			Description = ObjEvent.Description;
			StartTime = ObjEvent.StartDate.TimeOfDay;
			EndTime = ObjEvent.EndDate.TimeOfDay;
			StartDate = ObjEvent.StartDate;
			EndDate = ObjEvent.EndDate;

			if (ObjInternalEvent != null) { 
				
			}
		}

		public void UpdateModel()
		{
			if (IsNew)
			{ 
				ObjEvent = new GoogleEvent { ID=""};
				ObjInternalEvent = new Event
				{
					data = new Event.PersonalizedData
					{
						RelatedOwnerGuid= App.VM.user.Guid,
						State= Event.PersonalizedData.EventStateEnum.Inserted
					}
				};
			}

			ObjEvent.Name=Title;
			//Img = ObjEvent.ImageUris?.FirstOrDefault() ?? Globals.DefaultThumb;
			ObjEvent.Description=Description;
			ObjEvent.StartDate=StartDate.SetTime(StartTime);
			ObjEvent.EndDate=EndDate.SetTime(EndTime);
		}

		public async Task<GoogleEvent.UpdateResponse> UploadData()
		{
			IsLoadingData = true;
			try
			{
				var ws = new WebServiceV2();
				var config = new WebServiceV2.UrlToStringConfiguration
				{
					url = Globals.RestApiV1 + "loft/event/" + ObjEvent.ID + "/update",
					Type = WebServiceV2.UrlToStringConfiguration.RequestType.JsonRaw,
					Verb = WebServiceV2.UrlToStringConfiguration.RequestVerb.POST,
					RawContent = JsonConvert.SerializeObject(ObjEvent)
				};
				var res = await ws.UrlToString(config);
				var resObj= JsonConvert.DeserializeObject<GoogleEvent.UpdateResponse>(res);
				if (resObj != null) {
					this.ObjEvent = resObj.item;
					if (IsNew)
					{
						this.ObjInternalEvent.data.RelatedGoogleEventID = this.ObjEvent.ID;
						await this.ObjInternalEvent.update();
						IsNew = false;
					}//qui non ci posso mettere l'else
				}
				return resObj;
			}
			finally {
				IsLoadingData = false;
			}
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
