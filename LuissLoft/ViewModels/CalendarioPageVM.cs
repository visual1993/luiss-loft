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

namespace LuissLoft
{
	public class CalendarioPageVM : ViewModelBase
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
		public CalendarioPageVM()
		{
		}

		public async Task DownloadData()
		{
			IsLoadingData = true;
			IsLoadingData = false;
		}
		public void UpdateVM() { }

		/*private string password = "";
		public string Password { get { return password; } set { password = value; this.RaisePropertyChanged(); } }
		*/

	}
}
