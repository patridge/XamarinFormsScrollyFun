using System;
using System.Linq;
using System.Drawing;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
using Point = Xamarin.Forms.Point;
using Rectangle = Xamarin.Forms.Rectangle;

namespace ScrollyFun
{
	public class App : Application
	{
		public class MovableLabel : Label {
			public Rectangle OriginalPosition { get; set; }
			public bool MovingFunky { get; set; }
			public Point CalculateNewPosition(Rectangle parentBounds, double yDelta) {
				// TODO: try flipping upside down and making lighter from top to "roll over and behind".
				double newTop;
				double newLeft = Bounds.Left;
				if (!MovingFunky) {
					newTop = Bounds.Top + yDelta;
					if (newTop < 0) {
						MovingFunky = true;
						newLeft -= -newTop;
						newTop = 0;
						if (newLeft < 0) {
							newTop -= -newLeft;
							newLeft = 0;
						}
					}
				} else {
					newTop = Bounds.Top - yDelta;
				}

				var newLocation = new Point (newLeft, newTop);
				return newLocation;


				//				if (y > 0) {
				//					// Scrolling content up
				//					var newlyAffectedWordLabels = wordLabels.Where(label => !label.MovingFunky && label.Y <= 0);
				//					foreach (var newlyAffectedWordLabel in newlyAffectedWordLabels) {
				//						var newBounds = newlyAffectedWordLabel.Bounds;
				//						newBounds.Top = 0;
				//						AbsoluteLayout.SetLayoutBounds(newlyAffectedWordLabel, newBounds);
				//						newlyAffectedWordLabel.MovingFunky = true;
				//					}
				//					//					var movingWordLabels = wordLabels.Where(label => label.MovingFunky);
				//
				//				}
				//				else {
				//					// Scrolling content down
				//					// TODO: Reverse everything
				//				}

				//foreach (var wordLabel in affectedWordLabels) {
				// if at 0
				//   if left of center, flag to move left
				//   if right of center (or center exactly), flag to move right
				// move to side until 0/width, then start moving down (possibly rotate)
				//   (could also animate drop off screen at this point)
				// move down until height, then start moving back in to stack
				// (once stacked, possibly recycle at end of text)
				//}
			}
		}

		AbsoluteLayout funkyAbsoluteLayout;
		public App ()
		{
			// Sample text from Treasure Island (Stevenson, 1894).
			var sampleText = "SQUIRE TRELAWNEY, Dr. Livesey, and the rest of these gentlemen having asked me to write down the whole particulars about Treasure Island, from the beginning to the end, keeping nothing back but the bearings of the island, and that only because there is still treasure not yet lifted, I take up my pen in the year of grace 17__ and go back to the time when my father kept the Admiral Benbow inn and the brown old seaman with the sabre cut first took up his lodging under our roof.";
			var wordLabels = sampleText.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select (word => new MovableLabel { Text = word }).ToList ();
			funkyAbsoluteLayout = new AbsoluteLayout ();
			foreach (var wordLabel in wordLabels) {
				funkyAbsoluteLayout.Children.Add (wordLabel);
			};
			const double lineLeading = 3;
			const double wordSpacing = 3;
			funkyAbsoluteLayout.SizeChanged += (sender, e) => {
				// Lay out a bunch of text manually, because YOLO and such.
				double x = 0;
				double y = 0;
				foreach (var wordLabel in wordLabels) {
					if (x + wordLabel.Bounds.Width > funkyAbsoluteLayout.Width) {
						// NOTE: totally fails to wrap words longer than `funkyAbsoluteLayout.Width`.
						y += wordLabel.Bounds.Height + lineLeading;
						x = 0;
					}
					var newBounds = new Rectangle(new Point(x, y), wordLabel.Bounds.Size);
					wordLabel.OriginalPosition = newBounds;
					AbsoluteLayout.SetLayoutBounds(wordLabel, wordLabel.OriginalPosition);
					x = newBounds.Right + wordSpacing;
				}
			};

			var touchesContentPage = new TouchesContentPage {
				Content = funkyAbsoluteLayout,
				Padding = new Thickness(5, Device.OnPlatform(iOS: 25, Android: 5, WinPhone: 5), 5, 5),
				BackgroundColor = Color.Aqua,
			};
			touchesContentPage.OnPanned += (object sender, SizeF e) => {
				var y = e.Height;
				if (y > 0) {
					// Swiping down: currently ignored.
					return;
				}
				// TODO: Parallel.ForEach?
				foreach (var wordLabel in wordLabels) {
					var newLocation = wordLabel.CalculateNewPosition(funkyAbsoluteLayout.Bounds, y);
					AbsoluteLayout.SetLayoutBounds(wordLabel, new Rectangle(newLocation, wordLabel.Bounds.Size));
				}
			};
			MainPage = touchesContentPage;
		}
	}
}
