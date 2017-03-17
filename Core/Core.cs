using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Visual1993
{
	public class TextHelper
	{
		public enum StringType
		{
			PlainString = 0,
			Base64String = 1
		}
		public static string TimeDifferenceToString(DateTime i)
		{
			if (i == DateTime.MinValue || i == DateTime.MaxValue || i.Year < 1970) { return ""; }
			var diff = (DateTime.Now - i);
			if (diff < TimeSpan.FromSeconds(5)) { return "now"; }
			if (diff < TimeSpan.FromMinutes(1)) { return diff.TotalSeconds.ToString("## 'secs'"); }
			if (diff < TimeSpan.FromHours(1)) { return diff.TotalMinutes.ToString("## 'mins'"); }
			if (diff < TimeSpan.FromDays(1)) { return diff.TotalHours.ToString("## 'hours'"); }
			if (diff < TimeSpan.FromDays(31)) { return diff.TotalDays.ToString("## 'days'"); }
			if (diff < TimeSpan.FromDays(365)) { return (diff.TotalDays / 31).ToString("## 'months'"); }
			if (diff >= TimeSpan.FromDays(365)) { return (diff.TotalDays / 365).ToString("## 'years'"); }
			return "";
		}
		public static bool IsNullOrWhiteSpace(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return true;
			}
			if (input.ToLower() == "null") { return true; }
			return false;
		}
	}
}