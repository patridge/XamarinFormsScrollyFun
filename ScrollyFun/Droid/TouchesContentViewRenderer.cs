using System.Drawing;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using System.Linq;
using View = Android.Views.View;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(ScrollyFun.TouchesContentView), typeof(ScrollyFun.Droid.TouchesContentViewRenderer))]
namespace ScrollyFun.Droid {
    public class TouchesContentViewRenderer : VisualElementRenderer<TouchesContentView> {
        int ConvertToDp(double pixels) {
            return (int)(pixels / Resources.DisplayMetrics.Density);
        }
        PointF ConvertToDp(PointF pixelsPoint) {
            return new PointF(ConvertToDp(pixelsPoint.X), ConvertToDp(pixelsPoint.Y));
        }
        public override bool OnTouchEvent(MotionEvent e) {
            var touches = Enumerable.Range(0, e.PointerCount).Select(i => {
                var pointerId = e.GetPointerId(i);
                // Android X/Y are based on full pixel count, not the density-independent pixels X.F works in.
                var androidLocation = new PointF(e.GetX(i), e.GetY(i));
                var formsLocation = ConvertToDp(androidLocation);
                return new IdentifiedTouch {
                    Id = pointerId,
                    Location = formsLocation,
                };
            }).ToArray();

            MotionEventActions action = e.Action & MotionEventActions.Mask;
            switch (action) {
            case MotionEventActions.Down:
            case MotionEventActions.PointerDown:
                Element.TouchesBegan(touches);
                break;
            case MotionEventActions.Move:
                Element.TouchesMoved(touches);
                break;
            case MotionEventActions.Up:
            case MotionEventActions.PointerUp:
                Element.TouchesEnded(touches);
                break;
            case MotionEventActions.Cancel:
                Element.TouchesCancelled(touches);
                break;
            }

            return true;
        }
    }
}
