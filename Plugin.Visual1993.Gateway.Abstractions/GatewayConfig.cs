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
	public static class Constants
	{
		public static string RestAPI = "";//"https://ssl.visual1993.com/disco1/v1/";
		public static string GatewayUrl = "";//"https://ssl.visual1993.com/disco1/v1/gateway.php";
		public static string GatewaySecureBlowfish = "";//disco1
		public static bool GatewayUseMultipleGuidsSystem = true;
		public static bool GatewayIsCacheEnabled = false; //cannot use cache
		public static int GatewayTimerRepeatMsec = 150;
		public static int GatewayThreadSleepMsec = 100;
		public static TimeSpan HTTPTimeout = TimeSpan.FromSeconds(10);
		public static TimeSpan HTTPTimeoutMini = TimeSpan.FromSeconds(4);
		public static TimeSpan TimeoutMicroPerToken = TimeSpan.FromMilliseconds(1500);
		public static TimeSpan TokenCheckPeriod = TimeSpan.FromSeconds(30);
		public static TimeSpan GatewayCachePeriod = TimeSpan.FromMinutes(5);
		public static DateTime LastTokenRequestDateTime = DateTime.MinValue; //non ha senso che sia istanziato perchè si riferisce a tutta l'app
		public static bool IsDownloadingToken = false;

		public static UserData.SocialProvider CurrentUserSocialProvider;
		public static string CurrentUserSocialID;
		public static string CurrentUserLastAccessToken;
		public static string CurrentUserUsername = "guest"; // is default for public
		public static string CurrentUserPassword="guestPassword"; // is default for public
		public static string CurrentUserToken;

		public static string RandomAppToken
		{
			get
			{
				Random r = new Random();
				try { return AppTokens[r.Next(0, AppTokens.Count - 1)]; }
				catch (Exception) { return AppTokens.First(); }
			}
		}
		public static List<string> AppTokens = new List<string> {
			"847d9224-db23-429a-9c21-68376e75a680",
			"15c8fd0d-1e18-49e5-959c-644ce130c68d",
			"10894ac7-63a5-434d-90fa-967a5e1628e1"
		};
	}
}
