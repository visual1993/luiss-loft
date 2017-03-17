//prima era chiamata MyClass
using System;
using System.IO;
using Newtonsoft.Json;
using Visual1993.Extensions;
using Visual1993.Data;
using Visual1993;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Plugin.Visual1993.Gateway.Abstractions
{	
	public abstract class Gateway<T, T2>
	{
		//public GatewayConfig Constants;
		//"T contains data, T2 contains the response list model"
		public Gateway()
		{}
		public event EventHandler<Type> ContentChanged;
		public void OnContentChanged(Type type = null)
		{
			EventHandler<Type> handler = ContentChanged;
			if (handler != null)
			{
				handler(null, type);
			}
		}
		[Display(Description = "Va impostata da una const")]
		public string remoteClassName { get; set; }
		public string remoteEndpoint { get; set; } = Constants.GatewayUrl;
		public Guid Guid { get; set; }
		private string guid
		{ //questo arriva da MySQL
			get
			{
				return Guid.ToString();
			}
			set
			{
				if (value != null && value.Length > 0)
				{
					Guid = Guid.Parse(guid);
				}
			}
		}
		public virtual /*abstract*/ T dataBasic
		{
			get; set;
		}

		public class AccessTokenExistsResponse : WebServiceV2.DefaultResponse
		{
			public UserData.SocialNetworkRESTResponse SocialResponse { get; set; }
		}

		public class Response : WebServiceV2.DefaultResponse
		{
			public Guid guid { get; set; } //returned object guid
		}
		public abstract class GenericItem
		{
			public Guid Guid { get; set; }
		}
		public class ResponseList<T1> : WebServiceV2.DefaultResponse
		{
			[JsonProperty("items")]
			public List<T1> _items = new List<T1>();
			[JsonIgnore]
			public List<T1> items
			{
				get { return _items ?? new List<T1>(); }
				set { _items = value ?? new List<T1>(); }
			}
			public JToken Extra { get; set; }
			public bool IsValidForAtLeastOneItem
			{
				get
				{
					if (this.state == WebServiceV2.WebRequestState.Ok && this.items.FirstOrDefault() != null)
					{ return true; }
					return false;
				}
			}
			//public IEnumerable<T1> items = new List<T1>();
		}

		public virtual async Task<Response> insert(object fullData)
		{
			var secureToken = GatewaySecure.GatewaySecure.GetToken(Constants.GatewaySecureBlowfish);
			if (TextHelper.IsNullOrWhiteSpace(remoteEndpoint) || TextHelper.IsNullOrWhiteSpace(remoteClassName))
			{
				return new Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "endpoint or remote class name not set" };
			}
			if (this.guid == null || this.guid.Length < 1 || this.guid == Guid.Empty.ToString())
			{
				this.Guid = Guid.NewGuid();
			}
			if (checkProprieta() == false)
			{
				return new Response { errorMessage = "no guid" };
			}

			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.HTTPTimeoutMini).Wait();
			}

			string url = remoteEndpoint + "?action=insert" + "&UserToken=" + Constants.CurrentUserToken + "&AppToken=" + Constants.RandomAppToken + "&secureToken=" + secureToken;
			List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("guid", this.guid),
				new KeyValuePair<string, string>("remoteClassName", remoteClassName),
				new KeyValuePair<string, string>("data",JsonConvert.SerializeObject(fullData)),
			};

			WebServiceV2 webRequest = new WebServiceV2();
			var result = await webRequest.UrlToString(url, pairs);
			if (result == null || result.Length < 1)
			{
				return new Response { errorMessage = "no response" };
			}
			Response obj;
			try
			{
				obj = JsonConvert.DeserializeObject<Response>(result);
			}
			catch (Exception ex)
			{

				if (ex.GetType() == typeof(TimeoutException))
				{
					return new Response { errorMessage = "timeout", state = WebServiceV2.WebRequestState.TimeOut };
				}
				if (ex.GetType() == typeof(TaskCanceledException))
				{
					return new Response { errorMessage = "task cancelled", state = WebServiceV2.WebRequestState.TaskCancelled };
				}
				Debug.WriteLine("Parsing exception. Result was:" + Environment.NewLine + result);
				return new Response { errorMessage = "unable to parse json" };
			}
			if (obj.errorMessage != null)
			{
				Debug.WriteLine(obj.errorMessage + Environment.NewLine);
			}
			if (obj.state == WebServiceV2.WebRequestState.InvalidUserToken)
			{
				if ((DateTime.Now - Constants.LastTokenRequestDateTime) > Constants.TokenCheckPeriod)
				{ //richiedi nuovo token e poi fai insert
					Constants.LastTokenRequestDateTime = DateTime.Now;
					var resToken = await AccessToken.getFromGeneric(Constants.CurrentUserSocialProvider, Constants.CurrentUserUsername, Constants.CurrentUserPassword, Constants.CurrentUserSocialID, Constants.CurrentUserLastAccessToken);
					if (resToken.items.Count == 0) { return new Response { state = WebServiceV2.WebRequestState.NotAuthorized, errorMessage = "unable to obtain access token" }; }
					return await insert(fullData);
				}
				else {
					return obj; //che sarà invalido
				}
			}
			else {
				return obj;
			}
		}
		public virtual async Task<Response> update(object fullData)
		{
			var secureToken = GatewaySecure.GatewaySecure.GetToken(Constants.GatewaySecureBlowfish);
			if (TextHelper.IsNullOrWhiteSpace(remoteEndpoint) || TextHelper.IsNullOrWhiteSpace(remoteClassName))
			{
				return new Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "endpoint or remote class name not set" };
			}
			if (checkProprieta() == false)
			{
				return new Response { errorMessage = "no guid" };
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.HTTPTimeoutMini).Wait();
			}
			string url = remoteEndpoint + "?action=update" + "&UserToken=" + Constants.CurrentUserToken + "&AppToken=" + Constants.RandomAppToken + "&secureToken=" + secureToken;
			List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("guid", this.guid),
				new KeyValuePair<string, string>("remoteClassName", remoteClassName),
				new KeyValuePair<string, string>("data",JsonConvert.SerializeObject(fullData)),
			};

			WebServiceV2 webRequest = new WebServiceV2();
			var result = await webRequest.UrlToString(url, pairs);
			if (result == null || result.Length < 1)
			{
				return new Response { errorMessage = "no response" };
			}
			Response obj = JsonConvert.DeserializeObject<Response>(result);
			if (obj.errorMessage != null)
			{
				Debug.WriteLine(obj.errorMessage + Environment.NewLine);
			}
			if (obj.state == WebServiceV2.WebRequestState.InvalidUserToken)
			{
				if ((DateTime.Now - Constants.LastTokenRequestDateTime) > Constants.TokenCheckPeriod)
				{ //richiedi nuovo token e poi fai insert
					Constants.LastTokenRequestDateTime = DateTime.Now;
					var resToken = await AccessToken.getFromGeneric(Constants.CurrentUserSocialProvider, Constants.CurrentUserUsername, Constants.CurrentUserPassword, Constants.CurrentUserSocialID, Constants.CurrentUserLastAccessToken);
					if (resToken.items.Count == 0) { return new Response { state = WebServiceV2.WebRequestState.NotAuthorized, errorMessage = "unable to obtain access token" }; }
					//Constants.CurrentUserToken = resToken.items.FirstOrDefault().data.AccessTokenValue;
					return await update(fullData);
				}
				else {
					return obj; //che sarà invalido
				}
			}
			else {
				return obj;
				OnContentChanged(null);
			}
		}

		public virtual async Task<Response> remove()
		{
			var secureToken = GatewaySecure.GatewaySecure.GetToken(Constants.GatewaySecureBlowfish);
			if (TextHelper.IsNullOrWhiteSpace(remoteEndpoint) || TextHelper.IsNullOrWhiteSpace(remoteClassName))
			{
				return new Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "endpoint or remote class name not set" };
			}
			if (checkProprieta() == false)
			{
				return new Response { errorMessage = "no guid" };
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
			}
			if (Constants.IsDownloadingToken)
			{ //delay
				System.Threading.Tasks.Task.Delay(Constants.HTTPTimeoutMini).Wait();
			}
			string url = remoteEndpoint + "?action=remove" + "&UserToken=" + Constants.CurrentUserToken + "&AppToken=" + Constants.RandomAppToken + "&secureToken=" + secureToken; ;
			List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("guid", this.guid),
				new KeyValuePair<string, string>("remoteClassName", remoteClassName),
			};

			WebServiceV2 webRequest = new WebServiceV2();
			var result = await webRequest.UrlToString(url, pairs);
			if (result == null || result.Length < 1)
			{
				return new Response { errorMessage = "no response" };
			}
			Response obj = JsonConvert.DeserializeObject<Response>(result);
			if (obj.errorMessage != null)
			{
				Debug.WriteLine(obj.errorMessage + Environment.NewLine);
			}
			if (obj.state == WebServiceV2.WebRequestState.InvalidUserToken)
			{
				if ((DateTime.Now - Constants.LastTokenRequestDateTime) > Constants.TokenCheckPeriod)
				{ //richiedi nuovo token e poi fai insert
					Constants.LastTokenRequestDateTime = DateTime.Now;
					var resToken = await AccessToken.getFromGeneric(Constants.CurrentUserSocialProvider, Constants.CurrentUserUsername, Constants.CurrentUserPassword, Constants.CurrentUserSocialID, Constants.CurrentUserLastAccessToken);
					if (resToken.items.Count == 0) { return new Response { state = WebServiceV2.WebRequestState.NotAuthorized, errorMessage = "unable to obtain access token" }; }
					Constants.CurrentUserToken = resToken.items.FirstOrDefault().data.AccessTokenValue;
					return await remove();
				}
				else {
					return obj; //che sarà invalido
				}
			}
			else {
				return obj;
			}
		}

		private bool checkProprieta()
		{
			if (this.guid == null || this.guid.Length < 1)
			{
#if DEBUG
				throw new ArgumentException("guid is invalid");
#endif
				return false;
			}
			return true;
		}

		public static async Task<ResponseList<T2>> getBase(Guid guid, string remoteEndpoint, string remoteClassName, string filterQuery, List<ListQuery> listsQuery)
		{
			try
			{
				var secureToken = GatewaySecure.GatewaySecure.GetToken(Constants.GatewaySecureBlowfish);
				if (TextHelper.IsNullOrWhiteSpace(remoteEndpoint) || TextHelper.IsNullOrWhiteSpace(remoteClassName))
				{
					return new ResponseList<T2> { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "endpoint or remote class name not set" };
				}
				var guidStr = "";
				if (guid == Guid.Empty)
				{
					guidStr = "any";
				}
				else { guidStr = guid.ToString(); }
				//string url = remoteEndpoint + "?action=get&remoteClassName="+remoteClassName+"&guid="+guidStr+"&filterQuery="+filterQuery;
				var parameters = new GetQuery { guid = guidStr, filter = filterQuery };
				parameters.lists = listsQuery;

				if (Constants.IsDownloadingToken)
				{ //delay
					System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
				}
				if (Constants.IsDownloadingToken)
				{ //delay
					System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
				}
				if (Constants.IsDownloadingToken)
				{ //delay
					System.Threading.Tasks.Task.Delay(Constants.HTTPTimeoutMini).Wait();
				}
				var UserToken = Constants.CurrentUserToken;
				string url = remoteEndpoint + "?action=get&remoteClassName=" + remoteClassName + "&UserToken=" + UserToken + "&AppToken=" + Constants.RandomAppToken + "&secureToken=" + secureToken;
				var pairs = new List<KeyValuePair<string, string>>();
				pairs.Add(new KeyValuePair<string, string>("parameters", JsonConvert.SerializeObject(parameters)));

				WebServiceV2 webRequest = new WebServiceV2();
				var result = await webRequest.UrlToString(url, pairs);
				if (result == null || result.Length < 1)
				{
					return new ResponseList<T2> { errorMessage = "no response", state = WebServiceV2.WebRequestState.TimeOut };
				}
				ResponseList<T2> obj = new ResponseList<T2>();
				try
				{
					obj = JsonConvert.DeserializeObject<ResponseList<T2>>(result);
					if (obj.errorMessage != null)
					{
						Debug.WriteLine(obj.errorMessage + Environment.NewLine);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					return new ResponseList<T2> { state = WebServiceV2.WebRequestState.GenericError };
				}

				if (obj.state == WebServiceV2.WebRequestState.InvalidUserToken)
				{
					Debug.WriteLine("This token is invalid: " + UserToken);
					if ((DateTime.Now - Constants.LastTokenRequestDateTime) > Constants.TokenCheckPeriod)
					{ //richiedi nuovo token e poi fai insert
						Constants.LastTokenRequestDateTime = DateTime.Now;
						var resToken = await AccessToken.getFromGeneric(Constants.CurrentUserSocialProvider, Constants.CurrentUserUsername, Constants.CurrentUserPassword, Constants.CurrentUserSocialID, Constants.CurrentUserLastAccessToken);
						if (resToken.items.Count == 0) { return new ResponseList<T2> { state = WebServiceV2.WebRequestState.NotAuthorized, errorMessage = "unable to obtain access token" }; }
						//Constants.CurrentUserToken = resToken.items.FirstOrDefault().data.AccessTokenValue;
						return await getBase(guid, remoteEndpoint, remoteClassName, filterQuery, listsQuery);
					}
					else {
						return obj; //che sarà invalido
					}
				}
				else {
					return obj;
				}
			}
			catch (Exception ex)
			{
				if (ex.GetType() == typeof(TimeoutException))
				{
					return new ResponseList<T2> { errorMessage = "timeout", state = WebServiceV2.WebRequestState.TimeOut };
				}
				if (ex.GetType() == typeof(TaskCanceledException))
				{
					return new ResponseList<T2> { errorMessage = "task cancelled", state = WebServiceV2.WebRequestState.TaskCancelled };
				}
				return new ResponseList<T2> { errorMessage = "exception", state = WebServiceV2.WebRequestState.GenericNetworkError };
			}

		}

		public static async Task<ResponseList<T2>> getAdvanced(IEnumerable<Guid> guids, List<string> fields = null, string filterQuery = "", List<ListQuery> listsQuery = null, string remoteEndpoint = null, string remoteClassName = "", string customAction = "getAdvanced", List<AdditionalParametersQuery> additionalParameters = null)
		{
			if (remoteEndpoint == null) { remoteEndpoint = Constants.RestAPI;}
			try
			{
				var secureToken = GatewaySecure.GatewaySecure.GetToken(Constants.GatewaySecureBlowfish);
				if (TextHelper.IsNullOrWhiteSpace(remoteEndpoint) || TextHelper.IsNullOrWhiteSpace(remoteClassName))
				{
					return new ResponseList<T2> { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "endpoint or remote class name not set" };
				}
				var fieldsStr = ""; if (fields == null) { fieldsStr = "*"; } else { fieldsStr = string.Join(",", fields); }
				var guidsListStr = new List<string>();
				//string url = remoteEndpoint + "?action=get&remoteClassName="+remoteClassName+"&guid="+guidStr+"&filterQuery="+filterQuery;
				var parameters = new AdvancedGetQuery { filter = filterQuery, additionalParameters = additionalParameters };
				parameters.lists = listsQuery;
				if (guids == null) { parameters.isAnyGuid = true; }
				else { foreach (Guid val in guids) { guidsListStr.Add(val.ToString()); } }
				parameters.guids = guidsListStr;

				string url = remoteEndpoint + "?action=" + customAction + "&remoteClassName=" + remoteClassName + "&UserToken=" + Constants.CurrentUserToken + "&AppToken=" + Constants.RandomAppToken + "&secureToken=" + secureToken;

				if (Constants.IsDownloadingToken)
				{ //delay
					System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
				}
				if (Constants.IsDownloadingToken)
				{ //delay
					System.Threading.Tasks.Task.Delay(Constants.TimeoutMicroPerToken).Wait();
				}
				if (Constants.IsDownloadingToken)
				{ //delay
					System.Threading.Tasks.Task.Delay(Constants.HTTPTimeoutMini).Wait();
				}

				WebServiceV2 webRequest = new WebServiceV2();
				var pairs = new List<KeyValuePair<string, string>>();
				pairs.Add(new KeyValuePair<string, string>("parameters", JsonConvert.SerializeObject(parameters)));
				var result = await webRequest.UrlToString(url, pairs);
				if (result == null || result.Length < 1)
				{
					return new ResponseList<T2> { errorMessage = "no response", state = WebServiceV2.WebRequestState.TimeOut };
				}
				ResponseList<T2> obj = JsonConvert.DeserializeObject<ResponseList<T2>>(result);
				if (obj.errorMessage != null)
				{
					Debug.WriteLine(obj.errorMessage + Environment.NewLine);
				}
				if (obj.state == WebServiceV2.WebRequestState.InvalidUserToken)
				{
					if ((DateTime.Now - Constants.LastTokenRequestDateTime) > Constants.TokenCheckPeriod)
					{ //richiedi nuovo token e poi fai insert
						Constants.LastTokenRequestDateTime = DateTime.Now;
						var resToken = await AccessToken.getFromGeneric(Constants.CurrentUserSocialProvider, Constants.CurrentUserUsername, Constants.CurrentUserPassword, Constants.CurrentUserSocialID, Constants.CurrentUserLastAccessToken);
						if (resToken.items.Count == 0) { return new ResponseList<T2> { state = WebServiceV2.WebRequestState.NotAuthorized, errorMessage = "unable to obtain access token" }; }
						return await getAdvanced(guids, fields, filterQuery, listsQuery, remoteEndpoint, remoteClassName, customAction, additionalParameters);
					}
					else {
						return obj; //che sarà invalido
					}
				}
				else {
					return obj;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("exception in getAdvanced: " + ex.Message);
				if (ex.GetType() == typeof(TimeoutException))
				{
					return new ResponseList<T2> { errorMessage = "timeout", state = WebServiceV2.WebRequestState.TimeOut };
				}
				if (ex.GetType() == typeof(TaskCanceledException))
				{
					return new ResponseList<T2> { errorMessage = "task cancelled", state = WebServiceV2.WebRequestState.TaskCancelled };
				}
				return new ResponseList<T2> { errorMessage = "exception", state = WebServiceV2.WebRequestState.GenericNetworkError };
			}
		}
	}
	public class GetUserFromTokenQuery
	{
		public string AccessToken;
	}

	public class GetTokenQuery
	{
		public string username { get; set; }
		public string hashedpassword { get; set; }
		public string passwordFieldName = "md5Password";
		public string usernameFieldName = "mail";
		public string SocialID = "";
		public string SocialLastAccessToken = "";
		public UserData.SocialProvider SocialProvider = UserData.SocialProvider.None;
	}
	public class GetQuery
	{
		public string guid { get; set; }
		public string filter { get; set; }
		public List<ListQuery> lists { get; set; } = new List<ListQuery>();
	}
	public class AdvancedGetQuery : GetQuery
	{
		public List<string> guids { get; set; } = new List<string>();
		public List<AdditionalParametersQuery> additionalParameters { get; set; } = new List<AdditionalParametersQuery>();
		public string fields { get; set; } = "*";
		public bool isAnyGuid { get; set; } = false;
	}
	public class ListQuery
	{
		public string table_name { get; set; }
		public string field_name { get; set; }
	}
	public class AdditionalParametersQuery
	{
		public string key { get; set; }
		public string value { get; set; }
	}
	public class AccessToken : Gateway<AccessToken.PersonalizedData, AccessToken>
	{
		public const string remoteClassNameConst = "access_token";
		public AccessToken()
		{
			this.remoteClassName = remoteClassNameConst;
		}
		public PersonalizedData data { get; set; } = new PersonalizedData();
		public class PersonalizedData
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string AccessTokenValue { get; set; } = Guid.Empty.ToString();
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Guid RelatedUserGuid { get; set; } = Guid.Empty;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public DateTime LastChange { get; set; }
		}
		public static async Task<ResponseList<AccessToken>> getFromGeneric(
			UserData.SocialProvider SocialProvider, string Username = "", string HashedPassword = "", string SocialProviderID = "", string SocialProviderAccessToken = ""
		)
		{
			Gateway<AccessToken.PersonalizedData, AccessToken>.ResponseList<AccessToken> res = new Gateway<PersonalizedData, AccessToken>.ResponseList<AccessToken>();
			switch (SocialProvider)
			{
				case UserData.SocialProvider.None:
					{
						res = await getFromUsernamePassword(Username, HashedPassword);
						break;
					}
				default:
					{
						res = await getFromSocial(SocialProviderID, SocialProviderAccessToken, SocialProvider);
						break;
					}
			}
			if (res.items.Count > 0)
			{
				Constants.CurrentUserToken = res.items.FirstOrDefault().data.AccessTokenValue;
			}
			return res;
		}
		public static async Task<ResponseList<AccessToken>> getFromUsernamePassword(string Username, string HashedPassword)
		{
			//TODO: controlla che restituisca solo gli utenti con SocialProvider=0
			if (Constants.IsDownloadingToken)
			{ return new Gateway<PersonalizedData, AccessToken>.ResponseList<AccessToken>(); }
			else { Constants.IsDownloadingToken = true; }

			var parameters = new GetTokenQuery { username = Username, hashedpassword = HashedPassword };
			string url = Constants.GatewayUrl+ "?action=getUserToken&AppToken=" + Constants.RandomAppToken;
			var pairs = new List<KeyValuePair<string, string>>();
			pairs.Add(new KeyValuePair<string, string>("parameters", JsonConvert.SerializeObject(parameters)));

			WebServiceV2 webRequest = new WebServiceV2();
			var result = await webRequest.UrlToString(url, pairs, false, Constants.HTTPTimeoutMini);
			Constants.IsDownloadingToken = false;
			if (result == null || result.Length < 1)
			{
				return new ResponseList<AccessToken> { errorMessage = "no response", state = WebServiceV2.WebRequestState.TimeOut };
			}
			ResponseList<AccessToken> obj = new ResponseList<AccessToken>();
			try
			{
				obj = JsonConvert.DeserializeObject<ResponseList<AccessToken>>(result);
				if (obj.errorMessage != null)
				{
					Debug.WriteLine(obj.errorMessage + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return new ResponseList<AccessToken> { state = WebServiceV2.WebRequestState.GenericError };
			}
			return obj;
		}
		[Display(Description="Restituisce 403 se non trova utenti associati")]
		public static async Task<ResponseList<AccessToken>> getFromSocial(string SocialID, string SocialLastAccessToken, UserData.SocialProvider SocialProvider)
		{
			//TODO: controlla che restituisca solo gli utenti con SocialProvider<>0
			if (Constants.IsDownloadingToken)
			{ return new Gateway<PersonalizedData, AccessToken>.ResponseList<AccessToken>(); }
			else { Constants.IsDownloadingToken = true; }

			var parameters = new GetTokenQuery
			{
				SocialID = SocialID,
				SocialLastAccessToken = SocialLastAccessToken,
				SocialProvider = SocialProvider,
			};
			string url = Constants.GatewayUrl + "?action=getUserToken&AppToken=" + Constants.RandomAppToken;
			var pairs = new List<KeyValuePair<string, string>>();
			pairs.Add(new KeyValuePair<string, string>("parameters", JsonConvert.SerializeObject(parameters)));

			WebServiceV2 webRequest = new WebServiceV2();
			var result = await webRequest.UrlToString(url, pairs, false, Constants.HTTPTimeoutMini);
			Constants.IsDownloadingToken = false;
			if (result == null || result.Length < 1)
			{
				return new ResponseList<AccessToken> { errorMessage = "no response", state = WebServiceV2.WebRequestState.TimeOut };
			}
			ResponseList<AccessToken> obj = new ResponseList<AccessToken>();
			try
			{
				obj = JsonConvert.DeserializeObject<ResponseList<AccessToken>>(result);
				if (obj.errorMessage != null)
				{
					Debug.WriteLine(obj.errorMessage + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return new ResponseList<AccessToken> { state = WebServiceV2.WebRequestState.GenericError };
			}
			return obj;
		}
		[Display(Description="Restituisce 404 se non trova utenti associati")]
		public async Task<AccessTokenExistsResponse> esisteUserFromSocialToken(string SocialToken, UserData.SocialProvider SocialProvider)
		{
			var parameters = new GetTokenQuery
			{
				SocialLastAccessToken = SocialToken,
				SocialProvider = SocialProvider,
			};
			string url = Constants.GatewayUrl + "?action=esisteUserFromSocialToken&AppToken=" + Constants.RandomAppToken;
			var pairs = new List<KeyValuePair<string, string>>();
			pairs.Add(new KeyValuePair<string, string>("parameters", JsonConvert.SerializeObject(parameters)));

			WebServiceV2 webRequest = new WebServiceV2();
			var result = await webRequest.UrlToString(url, pairs, false, Constants.HTTPTimeoutMini);
			Constants.IsDownloadingToken = false;
			if (result == null || result.Length < 1)
			{
				return new AccessTokenExistsResponse { errorMessage = "no response", state = WebServiceV2.WebRequestState.TimeOut };
			}
			AccessTokenExistsResponse obj = new AccessTokenExistsResponse();
			try
			{
				obj = JsonConvert.DeserializeObject<AccessTokenExistsResponse>(result);
				if (obj.errorMessage != null)
				{
					Debug.WriteLine(obj.errorMessage + Environment.NewLine);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return new AccessTokenExistsResponse { state = WebServiceV2.WebRequestState.GenericError };
			}
			return obj;
		}

		[Obsolete("Not possible to insert a token client side")]
		public override async Task<Response> insert(object fullData = null)
		{
			return new Gateway<PersonalizedData, AccessToken>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "not possible from client side" };
		}
		[Obsolete("Not possible to update a token client side")]
		public override async Task<Response> update(object fullData = null)
		{
			return new Gateway<PersonalizedData, AccessToken>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "not possible from client side" };
		}
		[Obsolete("Not possible to remove a token client side")]
		public override async Task<Response> remove()
		{
			return new Gateway<PersonalizedData, AccessToken>.Response { state = WebServiceV2.WebRequestState.GenericError, errorMessage = "not possible from client side" };
		}
		public bool checkProperties()
		{
			return true;
		}
	}

	public class RemovedObjectClass
	{
		public bool IsRemoved = false;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Guid RelatedUserGuidWhoRemoved = Guid.Empty;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime DateRemoved = DateTime.MinValue;
	}
}
