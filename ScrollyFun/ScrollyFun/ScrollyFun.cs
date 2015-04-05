using System;
using System.Linq;
using System.Drawing;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
using Point = Xamarin.Forms.Point;
using Rectangle = Xamarin.Forms.Rectangle;
using System.Collections.Generic;

namespace ScrollyFun
{
	public class MoveDetails {
		public Rectangle ParentBounds { get; set; }
		public Rectangle CurrentBounds { get; set; }
		public int MoveIndex { get; set; }
		public SizeF DeltaMovement { get; set; }
	}
	public class App : Application
	{
		//foreach (var wordLabel in affectedWordLabels) {
		// if at 0
		//   if left of center, flag to move left
		//   if right of center (or center exactly), flag to move right
		// move to side until 0/width, then start moving down (possibly rotate)
		//   (could also animate drop off screen at this point)
		// move down until height, then start moving back in to stack
		// (once stacked, possibly recycle at end of text)
		//}
		// TODO: try flipping upside down and making lighter from top to "roll over and behind".
		public class MovableLabel : Label {
			int moveIndex = 0;

			/// <summary>
			/// A no-op move that never finishes (returns false to indicate it is not finished).
			/// </summary>
			static Func<MoveDetails, MoveDetails> NoMove = (moveDetails) => {
				return moveDetails;
			};

			public MovableLabel(Func<MoveDetails, MoveDetails> mover) {
				Mover = mover ?? NoMove;
			}
			protected Func<MoveDetails, MoveDetails> Mover { get; set; }
			public MoveDetails CalculateNewPosition(MoveDetails moveDetails) {
				moveDetails.MoveIndex = moveIndex;
				var newMoveDetails = Mover (moveDetails);
				moveIndex = newMoveDetails.MoveIndex;
				return newMoveDetails;
			}
		}

		AbsoluteLayout funkyAbsoluteLayout;
		public App ()
		{
			// Sample text from Treasure Island (Stevenson, 1894).
			var sampleText = "SQUIRE TRELAWNEY, Dr. Livesey, and the rest of these gentlemen having asked me to write down the whole particulars about Treasure Island, from the beginning to the end, keeping nothing back but the bearings of the island, and that only because there is still treasure not yet lifted, I take up my pen in the year of grace 17__ and go back to the time when my father kept the Admiral Benbow inn and the brown old seaman with the sabre cut first took up his lodging under our roof.";
			Func<MoveDetails, MoveDetails> doSomeFunMoving = (moveDetails) => {
				double moveDelta = (moveDetails.DeltaMovement.Height < 0 ? moveDetails.DeltaMovement.Height : 0.0); // currently only move for up panning
				Point location;
				float width = (float)moveDetails.CurrentBounds.Width;
				SizeF remainingDelta;
				Rectangle bounds;
				while (moveDetails.DeltaMovement != SizeF.Empty) {
					bounds = moveDetails.CurrentBounds;
					location = bounds.Location;
					remainingDelta = moveDetails.DeltaMovement;
					switch (moveDetails.MoveIndex) {
					case 0:
						location.Y += moveDelta;
						if (location.Y < 0) {
							// Overshot destination; put the rest back; move on.
							remainingDelta.Height = (float)location.Y;
							location.Y = 0;
							moveDetails.MoveIndex += 1;
						}
						else {
							remainingDelta = SizeF.Empty;
						}
						break;
					case 1:
						var right = bounds.Right;
						var left = bounds.Left;
						var parentCenter = moveDetails.ParentBounds.Center.X;
						bool moveLeft = right < parentCenter || (parentCenter - left > right - parentCenter);
						if (moveLeft) {
							location.X += moveDelta;
						} else {
							location.X -= moveDelta;
						}
						if (location.X < 0 || location.X + width > moveDetails.ParentBounds.Right) {
							// Overshot destination; put the rest back; move on.
							if (location.X < 0) {
								remainingDelta.Height = (float)location.X;
								location.X = 0;
							} else {
								remainingDelta.Height = (float)moveDetails.ParentBounds.Right - width - (float)location.X;
								location.X = moveDetails.ParentBounds.Right - width;
							}
							moveDetails.DeltaMovement = remainingDelta;
							moveDetails.MoveIndex += 1;
						}
						else {
							remainingDelta = SizeF.Empty;
						}
						break;
					case 2:
						location.Y -= moveDelta;
						if (location.Y > moveDetails.ParentBounds.Height) {
							// We're done now; zero out to stop loop.
							remainingDelta = SizeF.Empty;
							moveDetails.MoveIndex += 1;
						}
						else {
							remainingDelta = SizeF.Empty;
						}
						break;
					default:
						remainingDelta = SizeF.Empty;
						break;
					}
					bounds.Location = location;
					moveDetails.DeltaMovement = remainingDelta;
					moveDetails.CurrentBounds = bounds;
				}
				return moveDetails;
			};

			var wordLabels = sampleText.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select (word => new MovableLabel (doSomeFunMoving) { Text = word, }).ToList ();
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
					AbsoluteLayout.SetLayoutBounds(wordLabel, newBounds);
					x = newBounds.Right + wordSpacing;
				}
			};

			var touchesContentPage = new TouchesContentPage {
				Content = funkyAbsoluteLayout,
				Padding = new Thickness(5, Device.OnPlatform(iOS: 25, Android: 5, WinPhone: 5), 5, 5),
				BackgroundColor = Color.Aqua,
			};
			touchesContentPage.OnPanned += (object sender, SizeF e) => {
				// TODO: Parallel.ForEach?
				var parentBoundsWithoutPadding = touchesContentPage.Bounds;
				parentBoundsWithoutPadding.Height -= touchesContentPage.Padding.Bottom + touchesContentPage.Padding.Top;
				parentBoundsWithoutPadding.Width -= touchesContentPage.Padding.Right + touchesContentPage.Padding.Left;
				foreach (var wordLabel in wordLabels) {
					var moveDetails = new MoveDetails {
						ParentBounds = parentBoundsWithoutPadding,
						CurrentBounds = wordLabel.Bounds,
						DeltaMovement = e,
					};
					var resultMoveDetails = wordLabel.CalculateNewPosition(moveDetails);
					AbsoluteLayout.SetLayoutBounds(wordLabel, resultMoveDetails.CurrentBounds);
				}
			};
			MainPage = touchesContentPage;
		}
	}
}
