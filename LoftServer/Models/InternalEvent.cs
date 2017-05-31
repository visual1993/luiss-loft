using System;
using Plugin.Visual1993.Gateway.Abstractions;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Visual1993.Data;
using System.Threading.Tasks;
using System.Linq;
using Visual1993.Extensions;
using System.Text;

namespace LoftServer
{
	public class InternalEvent : Gateway<InternalEvent.PersonalizedData, InternalEvent>
	{
		public const string remoteClassNameConst = "event";
		public InternalEvent()
		{
			remoteClassName = remoteClassNameConst;
		}
		public PersonalizedData data { get; set; } = new PersonalizedData();
		public class PersonalizedData
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string RelatedGoogleEventID { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Guid RelatedOwnerGuid { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public object Image { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime LastChange { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public EventStateEnum State { get; set; } = EventStateEnum.Inserted;

			public enum EventStateEnum { Pending = -1, Inserted = 0, Approved = 2, Rejected = 3, Removed = 1 };
		}
		public CachedDataClass CachedData { get; set; } = new CachedDataClass();
		//should be ignored in online saving. Only token into consideration while inserting/updating the cache, because it serializes the fullObject
		public class CachedDataClass
		{
		}
		public static async Task<ResponseList<InternalEvent>> getAllFromGoogleID(string id)
		{
			return await get(Guid.Empty, "AND JSON_EXTRACT(|table_full_name|.data, '$." + nameof(PersonalizedData.RelatedGoogleEventID) + "')='" + id + "'");
		}
		public static async Task<ResponseList<InternalEvent>> getAllFromGoogleIDs(IEnumerable<string> ids)
		{
			return await get(Guid.Empty, "AND JSON_UNQUOTE(JSON_EXTRACT(|table_full_name|.data, '$." + nameof(PersonalizedData.RelatedGoogleEventID) + "')) IN ('" + string.Join("','", ids.Distinct()) + "') ");
		}
		public static async Task<ResponseList<InternalEvent>> getMultiple(List<Guid> guids)
		{
			return await getAdvanced(guids, null, "", null, Constants.GatewayUrl, remoteClassNameConst) ?? new Gateway<PersonalizedData, InternalEvent>.ResponseList<InternalEvent>();
		}
		public static async Task<ResponseList<InternalEvent>> getAll()
		{
			return await get(Guid.Empty) ?? new Gateway<PersonalizedData, InternalEvent>.ResponseList<InternalEvent>();
		}
		public static async Task<ResponseList<InternalEvent>> GetOne(Guid guid)
		{
			if (guid == Guid.Empty) { return new ResponseList<InternalEvent>(); }
			return await getBase(guid, Constants.GatewayUrl, remoteClassNameConst, "", null);
		}
		private static async Task<ResponseList<InternalEvent>> get(Guid guid, string filterQuery = "", List<ListQuery> listsQuery = null)
		{
			return await getBase(guid, Constants.GatewayUrl, remoteClassNameConst, filterQuery, listsQuery) ?? new Gateway<PersonalizedData, InternalEvent>.ResponseList<InternalEvent>();
		}
		public override async Task<Response> insert(object fullData = null)
		{
			if (checkProperties() == false)
			{
				return new Gateway<PersonalizedData, InternalEvent>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "validation failed" };
			}
			this.data.LastChange = DateTime.UtcNow;
			var res = await base.insert(this.data);
			sendMQTT();
			return res ?? new Gateway<PersonalizedData, InternalEvent>.Response();
		}
		public override async Task<Response> update(object fullData = null)
		{
			if (checkProperties() == false)
			{
				return new Gateway<PersonalizedData, InternalEvent>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "validation failed" };
			}
			this.data.LastChange = DateTime.UtcNow;
			var res = await base.update(this.data);
			sendMQTT();
			return res ?? new Gateway<PersonalizedData, InternalEvent>.Response();
		}
		public override async Task<Response> remove()
		{
			this.data.LastChange = DateTime.UtcNow;
			this.data.State = PersonalizedData.EventStateEnum.Removed;
			var res = await base.update(this.data);
			sendMQTT();
			return res ?? new Gateway<PersonalizedData, InternalEvent>.Response();
		}
		public void sendMQTT()
		{
		}
		public bool checkProperties()
		{
			return true;
		}
		public enum WhatToDownload { All = 1 }

	}
}
