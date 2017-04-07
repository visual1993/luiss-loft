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

using Xamarin.Forms;
namespace LuissLoft
{
	public class MasterPageVM : ViewModelBase
	{
		public HomePageMD MD { private get; set; }
		public NavigationPage Navigation
		{
			get { return MD?.Detail as NavigationPage; }
			set
			{
				MD.Detail = value;
				App.VM.Navigation = MD.Detail as NavigationPage;
				MD.IsPresented = false;
				this.RaisePropertyChanged();
			}
		}
		public MasterPageVM()
		{
			App.VM.PropertyChanged += (sender, e) => {
				if (e.PropertyName == nameof(AppViewModel.user) || e.PropertyName == nameof(AppViewModel.Settings)) {
					this.UpdateVM();
				}
			};
		}
		public async Task DownloadData()
		{
			IsLoadingData = true;
			IsLoadingData = false;
		}
		public void UpdateVM()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				if (App.VM.user != null)
				{
					IsLogged = true;
					UserName = App.VM.user.data.Nome;
					UserImg = App.VM.user.data.ImageUrl ?? Globals.DefaultThumb;
				}
				else {
					IsLogged = false;
					UserName = "";
					UserImg = Globals.DefaultThumb;
				}
			});
		}

		bool isLogged = false;
		public bool IsLogged
		{
			get { return isLogged; }
			set
			{
				isLogged = value;
				if (isLogged) { IsLoggedString = "Logout"; }
				else { IsLoggedString = "Login"; }
				this.RaisePropertyChanged();
			}
		}
		string isLoggedString = "Login";
		public string IsLoggedString
		{
			get { return isLoggedString; }
			set { isLoggedString = value; this.RaisePropertyChanged(); }
		}

		ImageSource userImg = Globals.DefaultThumb;
		public ImageSource UserImg
		{
			get { return userImg; }
			set { userImg = value; this.RaisePropertyChanged(); }
		}

		string userName = "";
		public string UserName
		{
			get { return userName; }
			set { userName = value; this.RaisePropertyChanged(); }
		}
	}
}
