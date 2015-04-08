using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Xamarin.Forms;

namespace ScrollyFun
{
	public class IdentifiedTouch {
		public object Id { get; set; }
		public PointF Location { get; set; }
	}
	public class TouchesContentView : ContentView {
		readonly Dictionary<object, IdentifiedTouch> CurrentTouches = new Dictionary<object, IdentifiedTouch>();

		public event EventHandler<SizeF> OnPanned;
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesBegan;
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesMoved;
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesEnded;
		public event EventHandler<IEnumerable<IdentifiedTouch>> OnTouchesCancelled;

		public virtual void TouchesBegan(IEnumerable<IdentifiedTouch> points)
		{
			// Save touch point for later use.
			foreach (var point in points) {
				if (!CurrentTouches.ContainsKey (point.Id)) {
					CurrentTouches.Add (point.Id, point);
				}
			}

			if (OnTouchesBegan != null) {
				OnTouchesBegan (this, points);
			}
		}
		public virtual void TouchesMoved(IEnumerable<IdentifiedTouch> points)
		{
			if (points.Count() == 1) {
				// Considering a pan to be a single finger moving.
				var currentPoint = points.First();
				var currentLocation = currentPoint.Location;
				IdentifiedTouch lastKnownTouch = null;
				CurrentTouches.TryGetValue (currentPoint.Id, out lastKnownTouch);
				if (lastKnownTouch != null) {
					var lastPoint = lastKnownTouch.Location;
					OnPanned (this, new SizeF (currentLocation.X - lastPoint.X, currentLocation.Y - lastPoint.Y));
					lastKnownTouch.Location = currentLocation;
				}
			}

			if (OnTouchesMoved != null) {
				OnTouchesMoved (this, points);
			}
		}
		public virtual void TouchesCancelled(IEnumerable<IdentifiedTouch> points)
		{
			foreach (var point in points) {
				CurrentTouches.Remove (point);
			}

			if (OnTouchesCancelled != null) {
				OnTouchesCancelled (this, points);
			}
		}
		public virtual void TouchesEnded(IEnumerable<IdentifiedTouch> points)
		{
			foreach (var point in points) {
				CurrentTouches.Remove (point);
			}

			if (OnTouchesEnded != null) {
				OnTouchesEnded (this, points);
			}
		}
	}
}
