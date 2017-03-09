using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

using Plugin.Visual1993.Gateway.Abstractions;
using Xamarin.Forms;

namespace LuissLoft
{
	public class App : Application
	{
		public App()
		{
			// The root page of your application
			var button = new Button {Text="test" };
			button.Clicked += async(sender, e) => {
				await DoTest();
			};
			var content = new ContentPage
			{
				Title = "LuissLoft",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						button,
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Welcome to Xamarin Forms!"
						}
					}
				}
			};

			MainPage = new NavigationPage(content);
		}
		private void SetupGateway()
		{ 
			/*
			Constants.AppTokens=new List<string> {
			"847d9224-db23-429a-9c21-68376e75a680",
			"15c8fd0d-1e18-49e5-959c-644ce130c68d",
			"10894ac7-63a5-434d-90fa-967a5e1628e1"
			};
			*/
			//Constants.RestAPI = "https://ssl.visual1993.com/disco1/v1/";
			//Constants.GatewaySecureBlowfish = "disco1";

			Constants.AppTokens = new List<string> {
			"e2fc341b-5185-410f-8adb-5006a9a3f616",
			"0b0eb8a7-01ae-4556-913b-0cc51b6a8fc6",
			"78a9fc3e-9a72-4246-927a-92689cd86c70"
			};
			Constants.RestAPI = "https://ssl.visual1993.com/luissloft/v1/";
			Constants.GatewayUrl = Constants.RestAPI + "gateway.php";
			Constants.GatewaySecureBlowfish = "luissloft";
		}
		private async Task DoTest()
		{
			var model = new TestModel
			{
				Guid = Guid.NewGuid(),
				data=new TestModel.PersonalizedData { 
					Name="ciao"
				}
			};
			var resInsert = await model.insert();
			//var res = await disco1.Locale.getAll();
			Console.WriteLine(resInsert.state);
			//TODO: manca da salvare l'access token e da farne il retrieve all'accensione
			//per salvarlo, metti un propertychanged nella classe Constants e sottoscrivi da qui
			//occhio che è statica, quindi devi sottoscrivere da un delegato statico
		}
		protected override void OnStart()
		{
			// Handle when your app starts
			SetupGateway();
			//DoTest();
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
#if WINDOWS_UWP
    public class Console {
        public static void WriteLine(string i)
        {
            Debug.WriteLine(i);
        }
        public static void WriteLine(object i)
        {
            Debug.WriteLine(i);
        }
    }
#endif
}
