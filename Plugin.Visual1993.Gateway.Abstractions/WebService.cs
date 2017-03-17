using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Visual1993.Data
{
	public class WebServiceV2
	{
		public class DefaultResponse
		{
			public string errorMessage { get; set; } = "";
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public WebRequestState state { get; set; } = WebRequestState.ND;
		}

		public enum WebRequestMethod
		{
			Get = 1,
			Insert = 2,
			Update = 3,
			Delete = 4,
		}

		public enum WebRequestState
		{
			Ok = 200,
			NotFound = 404,
			NotAuthorized = 403,
			InvalidUserToken = 603,
			InvalidAppToken = 604,
			GenericError = 400,
			DuplicateExistsOnServer = 409,
			ND = 0,
			GenericNetworkError = 800,
			TimeOut = 801,
			TaskCancelled = 802,
		}

		public static List<string> findUrlFromString(string str)
		{
			var output = new List<string>();
			try
			{
				Regex linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.IgnoreCase);
				//str = "house home go www.monstermmorpg.com/index.php?ciao=1 nice hospital http://www.monstermmorpg.com this is incorrect url http://www.monstermmorpg.commerged continue";
				foreach (Match m in linkParser.Matches(str))
				{
					if (m.Value.Contains("http") == false)
					{
						output.Add(m.Value.Insert(0, "http://"));
					}
					else {
						output.Add(m.Value);
					}
					//Console.WriteLine (m.Value);
				}
			}
			catch (Exception ex)
			{
			}
			return output;
		}

		public async Task<string> UrlToString(string url, List<KeyValuePair<string, string>> pairs = null, bool useModernHTTPClient = false, TimeSpan timeout = default(TimeSpan))
		{
			string ParametersString = "null=null"; //da eliminare
			if (pairs == null) { }
			else
			{
				foreach (KeyValuePair<string, string> kv in pairs)
				{
					ParametersString = ParametersString.Insert(ParametersString.Length, "&" + kv.Key + "=" + kv.Value);
				}
			}
			System.Net.Http.HttpClient webClient = new System.Net.Http.HttpClient();
			//if (useModernHTTPClient) { webClient = new System.Net.Http.HttpClient(new ModernHttpClient.NativeMessageHandler()); }
			using (webClient)
			{
				if (timeout == default(TimeSpan)) { timeout = TimeSpan.FromSeconds(20); }
				webClient.Timeout = timeout;
				string result = "";
				try
				{

					if (pairs == null)
					{
						result = await webClient.GetStringAsync(new Uri(url));
					}
					else
					{
						webClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
						var response = await webClient.PostAsync(new Uri(url), new System.Net.Http.FormUrlEncodedContent(pairs));
						result = await response.Content.ReadAsStringAsync();
					}

					if (result == null) { Debug.WriteLine("HTTP nullo" + Environment.NewLine); return null; }
					//Debugger.Log(3,"HTTP",result+Environment.NewLine);
					if (result.Length < 1)
					{
						return null;
					}
					return result;
				}
				catch (Exception ex)
				{
					if (ex != null)
					{
						Debug.WriteLine("HTTPv2 eccezione: " + ex.Message + " URL=" + url);
					}
					return null;
				}
				finally
				{
					webClient.Dispose();
				}

			}
		}

		public event EventHandler RispostaRicevuta;
		protected virtual void OnRispostaRicevuta(RispostaRicevutaEventArgs e)
		{
			EventHandler handler = RispostaRicevuta;
			if (handler != null)
			{
				handler(this, e);
			}
		}
		public class RispostaRicevutaEventArgs : EventArgs
		{
			public string risposta { get; set; }
		}
	}
}