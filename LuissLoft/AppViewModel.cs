using System;
using Visual1993;
using Visual1993.Controls;
using Xamarin.Forms;
using Visual1993.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Visual1993.Extensions;
using System.Linq;
using System.Collections.Generic;

using PCLStorage;

namespace LuissLoft
{
	public class AppViewModel : ViewModelBase
	{
		public Core.FileSystem Filesystem;
		public AppViewModel()
		{
			Filesystem = new Core.FileSystem();
			SettingsClass.Load().ContinueWith(delegate {
					
			});
		}

		public User user
		{
			get { return settings.user; }
			set {
				if (settings == null) { settings = new SettingsClass();}
				settings.user = value;
				this.RaisePropertyChanged();
			}
		}

		private SettingsClass settings = new SettingsClass();
		public SettingsClass Settings
		{
			get { return settings; }
			set { settings = value; this.RaisePropertyChanged(); }
		}
		public NavigationPage Navigation;
	}
	public class SettingsClass
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string NotificationPlayerID;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string NotificationPushToken;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public User user = null;

		public async Task<bool> Save()
		{ //TODO: mettici il timer
			return await DoSave();
		}
		private async Task<bool> DoSave()
		{
			try
			{
				var file = await PCLStorage.FileSystem.Current.LocalStorage.CreateFileAsync("Settings.json", PCLStorage.CreationCollisionOption.ReplaceExisting);
				await FileExtensions.WriteAllTextAsync(file, JsonConvert.SerializeObject(this));
				//await App.VM.Filesystem.SaveTextAsync("Settings.json", JsonConvert.SerializeObject(this));
				return true;
			}
			catch
			{
				return false;
			}
		}
		public static async Task Load()
		{
			//popola settings
			//if (await App.VM.Filesystem.FileExists("Settings.json"))
			if (await FileSystem.Current.LocalStorage.CheckExistsAsync("Settings.json")== ExistenceCheckResult.FileExists)
			{
				//var str = await App.VM.Filesystem.LoadTextAsync("Settings.json");
				var file = await FileSystem.Current.LocalStorage.GetFileAsync("Settings.json");
				var str = await FileExtensions.ReadAllTextAsync(file);
				try
				{
					App.VM.Settings = JsonConvert.DeserializeObject<SettingsClass>(str);
				}
				catch (Exception) { }
			}
		}
	}
}

