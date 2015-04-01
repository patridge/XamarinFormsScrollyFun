using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;

namespace ScrollyFun
{
	public class App : Application
	{
		public App ()
		{
			var touchesContentPage = new TouchesContentPage {
				BackgroundColor = Color.Aqua,
			};
			touchesContentPage.OnTouchesMoved += (object sender, IEnumerable<IdentifiedTouch> e) => {
				System.Diagnostics.Debug.WriteLine(e.Count());
				System.Diagnostics.Debug.WriteLine(string.Join("\n", e.Select(t => t.Location).ToArray()));
			};
			MainPage = touchesContentPage;
		}
	}
}
