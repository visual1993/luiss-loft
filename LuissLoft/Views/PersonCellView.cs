using System;
using Visual1993;
using Visual1993.Controls;
using Visual1993.Extensions;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Visual1993.Data;

namespace LuissLoft
{
	public class PersonCellView : ViewCell
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var VM = BindingContext as CellViewModelBase;
			if (VM == null)
				return;
			ContextActions.Clear();
			if (VM.IsEditable)
			{
				var menuRemove = new MenuItem
				{
					Text = "Rimuovi",
					IsDestructive = true
				};
				menuRemove.Clicked += (sender, e) =>
				{
					var item = (CellViewModelBase)BindingContext; if (item == null) { return; }
				};
			}
		}

		public PersonCellView()
		{
			var layoutIntero = new Grid {
				RowDefinitions=new RowDefinitionCollection { new RowDefinition { Height= GridLength.Auto } },
				ColumnDefinitions=new ColumnDefinitionCollection { 
					new ColumnDefinition{Width=new GridLength(0.3, GridUnitType.Star)},
					new ColumnDefinition{Width=new GridLength(0.7, GridUnitType.Star)},
				}
			};
			var img = new Image { };
			img.Bind(nameof(CellViewModelBase.Thumb));
			layoutIntero.AddChild(img, 0, 0);

			var title = new Label { };
			title.Bind(nameof(CellViewModelBase.Title));
			layoutIntero.AddChild(title, 0, 1);

			layoutIntero.Margin = new Thickness(5);

			View = layoutIntero;
		}

	}
}
