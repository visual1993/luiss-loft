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


using Nancy;
using Nancy.Extensions;

namespace LoftServer
{
	public class LoginArgs { 
		public string username { get; set; }
		public string password { get; set; }
	}
	public class LoginREST: NancyModule
	{
		public LoginREST()
		{
			Post[MainClass.LoftPrefix + "login", runAsync: true] = async (dynamic arg1, System.Threading.CancellationToken arg2) =>
			 {
				 string eventID = arg1.id;
				 var inputStr = Request.Body.AsString();
				 var h = JsonConvert.DeserializeObject<LoginArgs>(inputStr);
				var res = await GetDataFromMoodle(h.username,h.password);
				return JsonConvert.SerializeObject(res);
			 };
		}
		CookieContainer Cookies = new CookieContainer();

		public async Task<User.PersonalizedData> GetDataFromMoodle(string username, string password)
		{
			await GetPage("http://learn.luiss.it/user/edit.php");

			var pars = new Dictionary<string, string>();
			//username = "";
			//password = "";
			pars.Add("username", username);
			pars.Add("password", password);
			pars.Add("rememberusername", "0");
			pars.Add("anchor", "");

			try
			{
				var doc = await PostPage("http://learn.luiss.it/login/index.php", pars);
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
				catch
				{
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		#region privateMethods
		public async Task<HtmlDocument> GetPage(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			Cookies = new CookieContainer();
			request.CookieContainer = Cookies;

			HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
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
			try
			{
				using (var handler = new NativeMessageHandler() { UseCookies = true, CookieContainer = Cookies })
				using (var client = new HttpClient(handler) { })
				{
					client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
					var content = new FormUrlEncodedContent(pars);
					var result = await client.PostAsync(url, content);
					//result.EnsureSuccessStatusCode();
					var html = await result.Content.ReadAsStringAsync();
					var doc = new HtmlDocument();
					doc.LoadHtml(html);
					return doc;
				}
			}
			catch (Exception ex)
			{
				Visual1993.Console.WriteLine(ex.Message);
				return null;
			}
			*/

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
			requestStream.Close();
			//System.Threading.Thread.Sleep(500);

			var responseRaw = request.GetResponse();
			HttpWebResponse response = (HttpWebResponse)responseRaw;
			var stream = response.GetResponseStream();

			//When you get the response from the website, the cookies will be stored
			//automatically in "_cookies".

			using (var reader = new StreamReader(stream))
			{
				string html = await reader.ReadToEndAsync();//ReadToEnd();
				var doc = new HtmlDocument();
				doc.LoadHtml(html);

				requestStream.Dispose();
				stream.Dispose();

				return doc;
			}
		}
		#endregion
	}
}
