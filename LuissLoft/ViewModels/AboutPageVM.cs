using System;
using Visual1993;
using Visual1993.Controls;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visual1993.Data;
using CommonClasses;
using Newtonsoft.Json;

namespace LuissLoft
{
	public class AboutPageVM: ViewModelBase
	{
		public ObservableCollection<UtenteCell> Utenti { get; } = new ObservableCollection<UtenteCell>();
		public List<User> UtentiObj = new List<User>();
		public AboutPageVM()
		{
			
		}
		public async Task DownloadData()
		{
			IsLoadingData = true;
			try { UtentiObj = (await User.getAllFromMail(new string[] { "luca.pisano@studenti.luiss.it", "sullivan.decarli@studenti.luiss.it", "livio.rogante@studenti.luiss.it", "francesco.scudo@studenti.luiss.it", "francesco.pedulla@studenti.luiss.it" })).items; }
			finally { IsLoadingData = false;}
		}
		public void UpdateVM()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				Utenti.Clear();
				foreach (var utente in UtentiObj.OrderBy(x=>x.data?.Cognome))
				{
					var cellVM = new UtenteCell
					{
						Title = utente.data.Nome + " " + utente.data.Cognome,
						Thumb = utente.data.ImageUrl
					};
					Utenti.Add(cellVM);
				}
			});
		}
	}
	public class UtenteCell : CellViewModelBase
	{ 
	}
}
