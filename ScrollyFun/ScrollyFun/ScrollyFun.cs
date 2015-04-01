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
			touchesContentPage.OnPanned += (object sender, SizeF e) => {
				System.Diagnostics.Debug.WriteLine("Panned: {0}", e);
			};
			MainPage = touchesContentPage;
		}
	}
}
