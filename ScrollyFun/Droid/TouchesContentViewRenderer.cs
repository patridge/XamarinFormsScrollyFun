using System.Drawing;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using System.Linq;
using View = Android.Views.View;

[assembly: ExportRenderer (typeof(ScrollyFun.TouchesContentView), typeof(ScrollyFun.Droid.TouchesContentViewRenderer))]
namespace ScrollyFun.Droid
{
	public class TouchesContentViewRenderer : VisualElementRenderer<TouchesContentView>
	{
		public override bool OnTouchEvent (MotionEvent e)
		{
			var touches = Enumerable.Range (0, e.PointerCount).Select (i => {
				var pointerId = e.GetPointerId(i);
				var location = new PointF {
					X = e.GetX(i),
					Y = e.GetY(i),
				};
				return new IdentifiedTouch {
					Id = pointerId,
					Location = location,
				};
			}).ToArray();

			MotionEventActions action = e.Action & MotionEventActions.Mask;
			switch (action) {
			case MotionEventActions.Down:
			case MotionEventActions.PointerDown:
				Element.TouchesBegan (touches);
				break;
			case MotionEventActions.Move:
				Element.TouchesMoved (touches);
				break;
			case MotionEventActions.Up:
			case MotionEventActions.PointerUp:
				Element.TouchesEnded (touches);
				break;
			case MotionEventActions.Cancel:
				Element.TouchesCancelled (touches);
				break;
			}

			return true;
		}
	}
}
