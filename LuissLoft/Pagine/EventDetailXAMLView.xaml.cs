using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace LuissLoft
{
	public partial class EventDetailXAMLView : ContentPage
	{
		EventDetailViewModel VM;
		public EventDetailXAMLView() { }
		public EventDetailXAMLView(EventDetailViewModel vm)
		{
			VM = vm ?? new EventDetailViewModel();
			BindingContext = VM;

			InitializeComponent();
		}
	}
}
