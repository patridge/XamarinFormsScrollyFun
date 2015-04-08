using System.Drawing;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(ScrollyFun.TouchesContentView), typeof(ScrollyFun.iOS.TouchesContentViewRenderer))]
namespace ScrollyFun.iOS {
    public class TouchesContentViewRenderer : VisualElementRenderer<TouchesContentView> {
        public TouchesContentViewRenderer() {
            MultipleTouchEnabled = true;
        }

        public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt) {
            base.TouchesBegan(touches, evt);

            if (Element == null) {
                return;
            }

            var touchList = touches.ToArray<UITouch>();

            Element.TouchesBegan(touchList.Select(t => {
                var location = t.LocationInView(t.View);
                return new IdentifiedTouch {
                    Id = t,
                    Location = new PointF {
                        X = (float)location.X,
                        Y = (float)location.Y,
                    },
                };
            }));
        }

        public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt) {
            base.TouchesMoved(touches, evt);

            if (Element == null) {
                return;
            }

            var touchList = touches.ToArray<UITouch>();

            Element.TouchesMoved(touchList.Select(t => {
                var location = t.LocationInView(t.View);
                return new IdentifiedTouch {
                    Id = t,
                    Location = new PointF {
                        X = (float)location.X,
                        Y = (float)location.Y,
                    },
                };
            }));
        }

        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt) {
            base.TouchesEnded(touches, evt);

            if (Element == null) {
                return;
            }

            var touchList = touches.ToArray<UITouch>();

            Element.TouchesEnded(touchList.Select(t => {
                var location = t.LocationInView(t.View);
                return new IdentifiedTouch {
                    Id = t,
                    Location = new PointF {
                        X = (float)location.X,
                        Y = (float)location.Y,
                    },
                };
            }));
        }

        public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt) {
            base.TouchesCancelled(touches, evt);

            if (Element == null) {
                return;
            }

            var touchList = touches.ToArray<UITouch>();

            Element.TouchesCancelled(touchList.Select(t => {
                var location = t.LocationInView(t.View);
                return new IdentifiedTouch {
                    Id = t,
                    Location = new PointF {
                        X = (float)location.X,
                        Y = (float)location.Y,
                    },
                };
            }));
        }
    }
}
