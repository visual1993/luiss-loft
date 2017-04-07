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

namespace Tester
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			var client = new WebClient();
			client.GetPage("http://learn.luiss.it/user/edit.php");
			var doc = client.PostPage("http://learn.luiss.it/login/index.php");
			var nome = doc.GetElementbyId("id_firstname").GetAttributeValue("value", "");
			var cognome = doc.GetElementbyId("id_lastname").GetAttributeValue("value", "");
			var email = doc.GetElementbyId("id_email").GetAttributeValue("value", "");
			var imgDiv = doc.GetElementbyId("fitem_id_currentpicture").ChildNodes[1].ChildNodes[0].ChildNodes[0];
			if (imgDiv.Attributes.Contains("src")) {
				var img = imgDiv.Attributes["src"].Value;
			}
		}

		/*
		 * Descendants("div").Where(d =>
	   d.Attributes.Contains("class")
	   &&
	   d.Attributes["class"].Value.Contains("userpicture")
			                                                                                   )
		 */

	}

	public class WebClient
	{
		//The cookies will be here.
		private CookieContainer Cookies = new CookieContainer();

		//In case you need to clear the cookies
		public void ClearCookies()
		{
			Cookies = new CookieContainer();
		}
		public void SetCookies()
		{
			/*
			 Cookies.Add(new Cookie
			{
				Name = "SESSc6617e94a4fddb3526e3f7162bacf6f2",
				Value = "hoa9uikm42c28pstvsoptfcbr4",
				Domain = ".luiss.it",
				Path = "/",

			});
			Cookies.Add(new Cookie
			{
				Name = "_ga",
				Value = "GA1.2.462069884.1449487453",
				Domain = ".luiss.it",
				Path = "/",

			});
			*/
		}

		public HtmlDocument GetPage(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.CookieContainer = Cookies;

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			var stream = response.GetResponseStream();

			//When you get the response from the website, the cookies will be stored
			//automatically in "_cookies".

			using (var reader = new StreamReader(stream))
			{
				string html = reader.ReadToEnd();
				var doc = new HtmlDocument();
				doc.LoadHtml(html);
				return doc;
			}
		}
		public HtmlDocument PostPage(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";

			Dictionary<string, string> pars = new Dictionary<string, string>();
			pars.Add("username","");
			pars.Add("password", "");
			pars.Add("rememberusername", "0");
			pars.Add("anchor", "");

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
			request.ContentLength = data.Length;

			Stream requestStream = request.GetRequestStream();
			requestStream.Write(data, 0, data.Length);
			requestStream.Close();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			var stream = response.GetResponseStream();

			//When you get the response from the website, the cookies will be stored
			//automatically in "_cookies".

			using (var reader = new StreamReader(stream))
			{
				string html = reader.ReadToEnd();
				var doc = new HtmlDocument();
				doc.LoadHtml(html);
				return doc;
			}
		}
	}
}
