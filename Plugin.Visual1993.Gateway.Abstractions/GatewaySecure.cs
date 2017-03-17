using System;
using System.Text;
using PCLCrypto;
using static PCLCrypto.WinRTCrypto;

namespace GatewaySecure
{
	public class GatewaySecure
	{
		public GatewaySecure()
		{
		}
		public static string GetToken(string input)
		{
			//15 minutes interval
			//manca di estrarre anno, mese e giorno sul php
			var hash = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5);

			var timeUTC = DateTime.UtcNow;
			string minuteStr = timeUTC.Minute.ToString();
			if (timeUTC.Minute < 15) { minuteStr = "15"; }
			if (timeUTC.Minute >= 15 && timeUTC.Minute < 30) { minuteStr = "30"; }
			if (timeUTC.Minute >= 30 && timeUTC.Minute < 45) { minuteStr = "45"; }
			if (timeUTC.Minute >= 45 && timeUTC.Minute <= 60) { minuteStr = "60"; }

			var utcString = timeUTC.Year + "_" + timeUTC.Month + "_" + timeUTC.Day + "-" + timeUTC.Hour+":"+minuteStr;
			string plainText = utcString + input;
			var hashed = hash.HashData(Encoding.UTF8.GetBytes(plainText));

			var hex = new StringBuilder(hashed.Length * 2);
			foreach (byte b in hashed)
			{
				hex.AppendFormat("{0:x2}", b);
			}
			var hashedString = hex.ToString();
			return hashedString;
		}

			
	}
}
