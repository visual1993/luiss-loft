﻿using System;
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
	public class HomePageMD : MasterDetailPage
	{
		public HomePageMD()
		{
			var masterVM = new MasterPageVM { MD=this};
			masterVM.DownloadData().ContinueWith((arg) => { masterVM.UpdateVM(); });
			Master = new MasterPage(masterVM);

			var eventsVM = new EventsPageVM();
			eventsVM.DownloadData().ContinueWith((arg) => { eventsVM.UpdateVM(); });
			Detail = new NavigationPage(new EventsPage(eventsVM));

			this.IsPresented = true;
		}
	}
	public class MasterPage : ContentPage
	{
		public MasterPageVM VM;
		public MasterPage(MasterPageVM vm) {
			VM = vm ?? new MasterPageVM();
			BindingContext = VM;
			Title = "Menu";
			Icon = "menu";

			var buttLogin = new Button
			{
				Command = new Command(async() =>
				{
					if (VM.IsLogged == false)
					{
						var pageVM = new LoginPageVM();
						pageVM.DownloadData().ContinueWith((arg) => { pageVM.UpdateVM(); });
						pageVM.Completed += async (object sender, User e) =>
						{
							//detail = profilePage
							GoToEventsPage();
						};
						VM.Navigation = new NavigationPage(new LoginPageView(pageVM));
					}
					else {
						//doing logout
						App.VM.user = null;
						await App.VM.Settings.Save();
					}
				})
			};
			//buttLogin.SetBinding(View.IsVisibleProperty, new Binding(nameof(MasterPageVM.IsLogged), BindingMode.Default, new NegateBooleanConverter()));
			buttLogin.SetBinding(Button.TextProperty, new Binding(nameof(MasterPageVM.IsLoggedString)));

			var userImg = new Image {
				HeightRequest=64, WidthRequest=64
			};
			userImg.Bind(nameof(MasterPageVM.UserImg));

			var userName = new Label { 
				HorizontalTextAlignment= TextAlignment.Center,
				HorizontalOptions= LayoutOptions.Center
			};
			userName.Bind(nameof(MasterPageVM.UserName),true);

			var buttonCalendario = new Button { 
				Text="Prenotazione"
			};
			buttonCalendario.Clicked+= delegate {
				var pageVM = new CalendarioPageVM();
				pageVM.DownloadData().ContinueWith(delegate {
					pageVM.UpdateVM();
				});
				VM.Navigation = new NavigationPage(new CalendarioPage(pageVM));
			};

			var buttonAbout = new Button
			{
				Text = "About"
			};
			buttonAbout.Clicked += (sender, e) => {
				var pageVM = new AboutPageVM();
				pageVM.DownloadData().ContinueWith(delegate
				{
					pageVM.UpdateVM();
				});
				VM.Navigation = new NavigationPage(new AboutPage(pageVM));
			};

			Content = new StackLayout {
				Children ={
					new BoxView{ HeightRequest=20, Color= Color.Transparent, BackgroundColor=Color.Transparent},
					userImg,
					userName,
					buttLogin,
					new Button{Text="Eventi", Command=new Command(() => {
						GoToEventsPage();
					})},
					buttonCalendario,
					buttonAbout,
				}
			};
		}
		private void GoToEventsPage()
		{
			var eventsVM = new EventsPageVM();
			eventsVM.DownloadData().ContinueWith((arg) => { eventsVM.UpdateVM(); });
			VM.Navigation = new NavigationPage(new EventsPage(eventsVM));
		}
	}
}

