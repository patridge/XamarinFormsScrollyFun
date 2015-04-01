using System;
using System.Collections.Generic;
using System.Drawing;
using Xamarin.Forms;

namespace ScrollyFun
{
	public class IdentifiedTouch {
		public object Id { get; set; }
		public PointF Location { get; set; }
	}
	public class TouchesContentPage : ContentPage
	{
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesBegan;
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesMoved;
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesEnded;
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesCancelled;

		public virtual void TouchesBegan(IEnumerable<IdentifiedTouch> points)
		{
			if (OnTouchesBegan != null) {
				OnTouchesBegan (this, points);
			}
		}
		public virtual void TouchesMoved(IEnumerable<IdentifiedTouch> points)
		{
			if (OnTouchesMoved != null) {
				OnTouchesMoved (this, points);
			}
		}
		public virtual void TouchesCancelled(IEnumerable<IdentifiedTouch> points)
		{
			if (OnTouchesCancelled != null) {
				OnTouchesCancelled (this, points);
			}
		}
		public virtual void TouchesEnded(IEnumerable<IdentifiedTouch> points)
		{
			if (OnTouchesEnded != null) {
				OnTouchesEnded (this, points);
			}
		}
	}
}
