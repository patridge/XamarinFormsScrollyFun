using System;
using System.Linq;
using Xamarin.Forms;

namespace ScrollyFun
{
	public class App : Application
	{
		public class MovableLabel : Label {
			public Rectangle OriginalPosition { get; set; }
			public bool MovingFunky { get; set; }
			public Point CalculateNewPosition(Rectangle parentBounds, double delta) {
				var newTop = Bounds.Top - delta;
				var newLeft = Bounds.Left;
				if (newTop < 0) {
					newLeft = -newTop;
					newTop = 0;
				}
				if (newLeft < 0) {
					newTop = -newLeft;
					newLeft = 0;
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
		ContentPage mainContentPage;
		AbsoluteLayout funkyAbsoluteLayout;
		public App ()
		{
			var sampleText = "asdf asdf asdf asdf asdf";
			var wordLabels = sampleText.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select (word => new MovableLabel { Text = word }).ToList ();
			funkyAbsoluteLayout = new AbsoluteLayout ();
			double lineLeading = 5;
			double wordSpacing = 3;
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
			foreach (var wordLabel in wordLabels) {
				funkyAbsoluteLayout.Children.Add (wordLabel);
			};
			var panRecognizerScrollView = new ScrollView {
				Content = funkyAbsoluteLayout,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				//BackgroundColor = Color.Olive,
			};
			panRecognizerScrollView.SizeChanged += (sender, e) => {
				// HACK: Trick the ScrollView into allowing us to scroll even if it's showing everything important already.
				// NOTE: If I ever have trouble with playing with this layout's height, I could try adding a separate view used only for this purpose.
				// NOTE: If I ever have trouble with the text scrolling (e.g., bounce), maybe just keep it separate and tie into the scrollview Scrolled anyway.
				funkyAbsoluteLayout.HeightRequest = panRecognizerScrollView.Height + 1;
			};
			panRecognizerScrollView.Scrolled += (object sender, ScrolledEventArgs e) => {
				// var x = e.ScrollX; // Swiping right/left: currently ignored.
				var y = e.ScrollY;
				System.Diagnostics.Debug.WriteLine ("ScrollY: {0}", y);
				if (y > 0) {
					// Swiping down: currently ignored.
					return;
				}

				// TODO: Parallel.ForEach?
				foreach (var wordLabel in wordLabels) {
					var newLocation = wordLabel.CalculateNewPosition(funkyAbsoluteLayout.Bounds, y);
					AbsoluteLayout.SetLayoutBounds(this, new Rectangle(newLocation, wordLabel.Bounds.Size));
				}
			};
			mainContentPage = new ContentPage {
				Content = panRecognizerScrollView,
				Padding = new Thickness(5, Device.OnPlatform(iOS: 25, Android: 5, WinPhone: 5), 5, 5),
			};

			MainPage = mainContentPage;
		}
	}
}