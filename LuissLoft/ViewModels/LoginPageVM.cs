using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using Newtonsoft.Json;
using Visual1993.Data;
using Visual1993;
using System.Threading.Tasks;

using ModernHttpClient;
using System.Net.Http;

namespace LuissLoft
{
	public class LoginPageVM : ViewModelBase
	{
		public event EventHandler<User> Completed;
		public virtual void OnCompleted(User user)
		{
			EventHandler<User> handler = Completed;
			if (handler != null)
			{
				handler(this, user);
			}
		}
		public LoginPageVM()
		{
		}
		public async Task<WebServiceV2.DefaultResponse> DoLogin(User.PersonalizedData d)
		{
			var output = new WebServiceV2.DefaultResponse();
			if (string.IsNullOrWhiteSpace(d.Email)) { return output;}
			var existingUser = (await User.getAllFromMail(d.Email)).items.FirstOrDefault();
			if (existingUser == null)
			{
				existingUser = new User
				{
					Guid = Guid.NewGuid(),
					data = d
				};
				await DoSalvaUtente(existingUser);
				return await existingUser.insert();
			}
			else {
				existingUser.data = d;
				await DoSalvaUtente(existingUser);
				return await existingUser.update();
			}
			return output;		   
		}
		private async Task DoSalvaUtente(User i)
		{
			App.VM.user = i;
			await App.VM.user.SaveOffline();
		}
		public static HttpClient GetHttpClient(IList<Cookie> cookies)
		{
			var messageHandler = new NativeMessageHandler();
			return new HttpClient(messageHandler);
		}
		public async Task<User.PersonalizedData> GetDataFromMoodle(string username, string password)
		{
			var pars = new Dictionary<string, string>();
			//username = "";
			//password = "";
			pars.Add("username", username);
			pars.Add("password", password);
			pars.Add("rememberusername", "0");
			pars.Add("anchor", "");

			var cookieHandler = new NativeCookieHandler();
			var messageHandler = new NativeMessageHandler(false, false, cookieHandler);
			var httpClient = new HttpClient(messageHandler);
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
			var content = new FormUrlEncodedContent(pars);
			var httpResponse =
				await httpClient.PostAsync(new Uri("http://learn.luiss.it/login/index.php"), content);

			var client = GetHttpClient(cookieHandler.Cookies);
			var res =
				await client.GetAsync(new Uri("http://learn.luiss.it/user/edit.php"));
			var strRes = await res.Content.ReadAsStringAsync();

			try
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(strRes);
				var nome = doc.GetElementbyId("id_firstname").GetAttributeValue("value", "");
				var cognome = doc.GetElementbyId("id_lastname").GetAttributeValue("value", "");
				var email = doc.GetElementbyId("id_email").GetAttributeValue("value", "");
				HtmlNode imgDiv = null;
				var img = "";
				try
				{
					imgDiv = doc.GetElementbyId("fitem_id_currentpicture").ChildNodes[1].ChildNodes[0].ChildNodes[0];
					if (imgDiv.Attributes.Contains("src"))
					{
						img = imgDiv.Attributes["src"].Value;
					}
				}
				catch { 
				}
				return new User.PersonalizedData
				{
					Nome = nome,
					Cognome = cognome,
					Email = email,
					ImageUrl = img,
					LastChange = DateTime.Now
				};
			}
			catch(Exception ex) {
				Visual1993.Console.WriteLine(ex.Message);
				return null;
			}

		}
		public async Task DownloadData()
		{
			IsLoadingData = true;
			IsLoadingData = false;
		}
		public void UpdateVM() { }

		private string username = "";
		public string Username { get { return username; } set { username = value; this.RaisePropertyChanged(); } }

		private string password = "";
		public string Password { get { return password; } set { password = value; this.RaisePropertyChanged(); } }

		CookieContainer Cookies = new CookieContainer();
		List<Cookie> cookies = new List<Cookie>();
		List<string> rawCookies = new List<string>();


		#region privateMethods
		public async Task<HtmlDocument> GetPage(string url)
		{
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			Cookies= new CookieContainer();
			request.CookieContainer = Cookies;

			HttpWebResponse response =  (HttpWebResponse)await request.GetResponseAsync();
			var stream = response.GetResponseStream();

			//When you get the response from the website, the cookies will be stored
			//automatically in "Cookies".

			using (var reader = new StreamReader(stream))
			{
				string html = reader.ReadToEnd();
				var doc = new HtmlDocument();
				doc.LoadHtml(html);
				return doc;
			}

		}

		public async Task<HtmlDocument> PostPage(string url, Dictionary<string, string> pars)
		{
			
			/*
			cookies = new List<Cookie>();
			rawCookies = new List<string>();
			var ck = Cookies.GetCookies(new Uri("http://learn.luiss.it/user/edit.php"));
			foreach (Cookie cookie in ck)
			{
				cookies.Add(cookie);
				rawCookies.Add(string.Format("{0}={1}", cookie.Name, cookie.Value));
			}

			var handler = new NativeMessageHandler() { UseCookies = false};
			var client = new HttpClient(handler);
			client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
			client.DefaultRequestHeaders.Add("Cookie", string.Join("; ",rawCookies));

			var content = new FormUrlEncodedContent(pars);
			var result = await client.PostAsync(url, content);
			//result.EnsureSuccessStatusCode();
			var html = await result.Content.ReadAsStringAsync();
			result.Content.c
			var doc = new HtmlDocument();
			doc.LoadHtml(html);
			return doc;
*/

			HttpWebRequest request = WebRequest.CreateHttp(url);
			request.Method = "POST";

			string postData = "";

			foreach (string key in pars.Keys)
			{
				postData += Uri.EscapeDataString(key) + "="
					  + Uri.EscapeDataString(pars[key]) + "&";
			}

			request.Method = "POST";
			request.CookieContainer = Cookies;

			byte[] data = Encoding.ASCII.GetBytes(postData);

			request.ContentType = "application/x-www-form-urlencoded";
			//request.Headers.c = data.Length;

			Stream requestStream = await request.GetRequestStreamAsync();
			await requestStream.WriteAsync(data, 0, data.Length);
			requestStream.Dispose();
			System.Threading.Thread.Sleep(5000);


			HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
			var stream = response.GetResponseStream();

			//When you get the response from the website, the cookies will be stored
			//automatically in "_cookies".

			using (var reader = new StreamReader(stream))
			{
				string html = await reader.ReadToEndAsync();//ReadToEnd();
				var doc = new HtmlDocument();
				doc.LoadHtml(html);
				return doc;
			}

		}
		#endregion
	}
}
