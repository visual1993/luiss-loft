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
	public class User : Gateway<User.PersonalizedData, User>
	{
		public const string remoteClassNameConst = "user";
		public User()
		{
			remoteClassName = remoteClassNameConst;
		}
		public PersonalizedData data { get; set; } = new PersonalizedData();
		public class PersonalizedData
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Nome { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Cognome { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Email { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ImageUrl { get; set; }

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime LastChange { get; set; }

		}
		public static async Task<ResponseList<User>> getAllFromMail(string mail)
		{
			return await get(Guid.Empty, "AND JSON_EXTRACT(|table_full_name|.data, '$." + nameof(PersonalizedData.Email) + "')='" + mail.ToString() + "'");
		}
		public static async Task<ResponseList<User>> getAllFromMail(IEnumerable<string> mails)
		{
			return await get(Guid.Empty, "AND JSON_UNQUOTE(JSON_EXTRACT(|table_full_name|.data, '$." + nameof(PersonalizedData.Email) + "')) IN ('" + string.Join("','", mails.Distinct()) + "') ");
		}
		public static async Task<ResponseList<User>> getMultiple(List<Guid> guids)
		{
			return await getAdvanced(guids, null, "", null, Constants.GatewayUrl, remoteClassNameConst) ?? new Gateway<PersonalizedData, User>.ResponseList<User>();
		}
		public static async Task<ResponseList<User>> getAll()
		{
			return await get(Guid.Empty) ?? new Gateway<PersonalizedData, User>.ResponseList<User>();
		}
		public static async Task<ResponseList<User>> GetOne(Guid guid)
		{
			if (guid == Guid.Empty) { return new ResponseList<User>(); }
			return await getBase(guid, Constants.GatewayUrl, remoteClassNameConst, "", null);
		}
		private static async Task<ResponseList<User>> get(Guid guid, string filterQuery = "", List<ListQuery> listsQuery = null)
		{
			return await getBase(guid, Constants.GatewayUrl, remoteClassNameConst, filterQuery, listsQuery) ?? new Gateway<PersonalizedData, User>.ResponseList<User>();
		}
		public override async Task<Response> insert(object fullData = null)
		{
			if (checkProperties() == false)
			{
				return new Gateway<PersonalizedData, User>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "validation failed" };
			}
			this.data.LastChange = DateTime.UtcNow;
			var res = await base.insert(this.data);
			sendMQTT();
			return res ?? new Gateway<PersonalizedData, User>.Response();
		}
		public override async Task<Response> update(object fullData = null)
		{
			if (checkProperties() == false)
			{
				return new Gateway<PersonalizedData, User>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "validation failed" };
			}
			this.data.LastChange = DateTime.UtcNow;
			var res = await base.update(this.data);
			sendMQTT();
			return res ?? new Gateway<PersonalizedData, User>.Response();
		}
		public override async Task<Response> remove()
		{
			this.data.LastChange = DateTime.UtcNow;
			var res = await base.remove();
			sendMQTT();
			return res ?? new Gateway<PersonalizedData, User>.Response();
		}
		public void sendMQTT()
		{
		}
		public bool checkProperties()
		{
			return true;
		}

	}
}
