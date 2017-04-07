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
using Visual1993.Controls;
using System.Threading.Tasks;

using Xamarin.Forms;
namespace LuissLoft
{
	public class LoginPageView: ContentPage
	{
		public LoginPageVM VM;
		public LoginPageView(LoginPageVM vm)
		{
			VM = vm ?? new LoginPageVM();
			BindingContext = VM;

			var user = new EntryWithLabelV2 { LabelText="Email" };
			user.Element.Keyboard = Keyboard.Email;
			user.Bind(nameof(LoginPageVM.Username));

			var pass = new EntryWithLabelV2 { LabelText = "Password" };
			pass.Element.IsPassword = true;	
			pass.Bind(nameof(LoginPageVM.Password));

			var loadingIndicator = new ActivityIndicator { };
			loadingIndicator.Bind(nameof(LoginPageVM.IsLoadingData));

			var buttLogin = new Button { Text = "Login" };
			buttLogin.Clicked+= async delegate {
				VM.IsLoadingData = true;
				try
				{
					var data = await VM.GetDataFromMoodle(VM.Username, VM.Password);
					if (data == null)
					{
						await App.Current.MainPage.DisplayAlert("Error", "Wrong login", "Retry");
						return;
					}
					var loginResult = await VM.DoLogin(data);
					if (loginResult.state != WebServiceV2.WebRequestState.Ok)
					{
						await App.Current.MainPage.DisplayAlert("Error", loginResult.errorMessage, "back");
					}
					else
					{
						//già ci pensa DoLogin a salvare
						VM.OnCompleted(App.VM.user);
					}
				}
				finally {
					VM.IsLoadingData = false;
				}
			};

			Content = new StackLayout { 
				Children = { 
					loadingIndicator,
					user,
					pass,
					buttLogin
				}
			};
		}
	}
}
