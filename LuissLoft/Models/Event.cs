using System;
using Plugin.Visual1993.Gateway.Abstractions;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Visual1993.Data;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using Visual1993.Extensions;
using System.Text;

namespace LuissLoft
{
	public class Event : Gateway<Event.PersonalizedData, Event>
	{
		public const string remoteClassNameConst = "event";
		public Event()
		{
			remoteClassName = remoteClassNameConst;
		}
		public PersonalizedData data { get; set; } = new PersonalizedData();
		public class PersonalizedData
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Name { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime LastChange { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public EventStateEnum State { get; set; } = EventStateEnum.Inserted;

			public enum EventStateEnum { Inserted = 0, Removed = 1 };
		}
		public CachedDataClass CachedData { get; set; } = new CachedDataClass();
		//should be ignored in online saving. Only token into consideration while inserting/updating the cache, because it serializes the fullObject
		public class CachedDataClass
		{ 
		}
		public static async Task<ResponseList<Event>> getMultiple(List<Guid> guids)
		{
			return await getAdvanced(guids, null, "", null, Constants.GatewayUrl, remoteClassNameConst)?? new Gateway<PersonalizedData, Event>.ResponseList<Event>();
		}
		public static async Task<ResponseList<Event>> getAll()
		{
			return await get(Guid.Empty)?? new Gateway<PersonalizedData, Event>.ResponseList<Event>();
		}
		public static async Task<ResponseList<Event>> GetOne(Guid guid)
		{
			if (guid == Guid.Empty) { return new ResponseList<Event>(); }
			return await getBase(guid, Constants.GatewayUrl, remoteClassNameConst, "", null);
		}
		private static async Task<ResponseList<Event>> get(Guid guid, string filterQuery = "", List<ListQuery> listsQuery = null)
		{
			return await getBase(guid, Constants.GatewayUrl, remoteClassNameConst, filterQuery, listsQuery)??new Gateway<PersonalizedData, Event>.ResponseList<Event>();
		}
		public override async Task<Response> insert(object fullData = null)
		{
			if (checkProperties() == false)
			{
				return new Gateway<PersonalizedData, Event>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "validation failed" };
			}
			this.data.LastChange = DateTime.UtcNow;
			var res = await base.insert(this.data);
			sendMQTT();
			return res?? new Gateway<PersonalizedData, Event>.Response();
		}
		public override async Task<Response> update(object fullData = null)
		{
			if (checkProperties() == false)
			{
				return new Gateway<PersonalizedData, Event>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "validation failed" };
			}
			this.data.LastChange = DateTime.UtcNow;
			var res = await base.update(this.data);
			sendMQTT();
			return res??new Gateway<PersonalizedData, Event>.Response();
		}
		public override async Task<Response> remove()
		{
			this.data.LastChange = DateTime.UtcNow;
			this.data.State = PersonalizedData.EventStateEnum.Removed;
			var res = await base.update(this.data);
			sendMQTT();
			return res??new Gateway<PersonalizedData, Event>.Response();
		}
		public void sendMQTT()
		{
		}
		public bool checkProperties()
		{
			return true;
		}
		public enum WhatToDownload { All = 1 }
		public async Task DownloadDataManually(WhatToDownload what = WhatToDownload.All) //otherwise it will slow the whole system as it doesn't go under gateway caching.
		{
			if (what == WhatToDownload.All)
			{
				OnContentChanged(null);
			}
		}
	}
}
