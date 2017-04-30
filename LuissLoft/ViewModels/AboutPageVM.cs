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
	public class AboutPageVM
	{
		public ObservableCollection<View> Foto { get; } = new ObservableCollection<View>();
		public List<User> Utenti = new List<User>();
		public AboutPageVM()
		{
			
		}
		public async Task DownloadData()
		{
			Utenti = (await User.getAllFromMail(new string[] { "luca.pisano@studenti.luiss.it", "sullivan.decarli@studenti.luiss.it", "livio.rogante@studenti.luiss.it", "francesco.scudo@studenti.luiss.it" })).items;

		}
		public void UpdateVM()
		{
			Foto.Clear();
			foreach (var utente in Utenti)
			{
				var image = new Image { Source = utente.data.ImageUrl };
				var nome = new Label{ Text=utente.data.Nome+" "+utente.data.Cognome};
				var stack = new StackLayout
				{
					Children = { image, nome}
				};
				Foto.Add(stack);
			}
		}
	}
}
