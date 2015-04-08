using System;
using System.Linq;
using System.Drawing;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
using Size = Xamarin.Forms.Size;
using Point = Xamarin.Forms.Point;
using Rectangle = Xamarin.Forms.Rectangle;
using System.Collections.Generic;
using ScrollyFun.Helpers;

namespace ScrollyFun {
    public class MoveDetails {
        public Rectangle ContainerBounds { get; set; }

        public Rectangle CurrentBounds { get; set; }

        public int MoveIndex { get; set; }

        public SizeF DeltaMovement { get; set; }
    }

    public class App : Application {
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
        public class MovableBoxView : BoxView {
            int moveIndex = 0;

            /// <summary>
            /// A no-op move that never finishes (returns false to indicate it is not finished).
            /// </summary>
            static Func<MoveDetails, MoveDetails> NoMove = (moveDetails) => {
                return moveDetails;
            };

            public MovableBoxView(Func<MoveDetails, MoveDetails> mover) {
                Mover = mover ?? NoMove;
            }

            protected Func<MoveDetails, MoveDetails> Mover { get; set; }

            public MoveDetails CalculateNewPosition(MoveDetails moveDetails) {
                moveDetails.MoveIndex = moveIndex;
                var newMoveDetails = Mover(moveDetails);
                moveIndex = newMoveDetails.MoveIndex;
                return newMoveDetails;
            }
        }

        static Random rand = new Random();
        AbsoluteLayout funkyAbsoluteLayout;

        public App() {
            var squareCount = 100;
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
                        var parentCenter = moveDetails.ContainerBounds.Center.X;
                        bool moveLeft = right < parentCenter || (parentCenter - left > right - parentCenter);
                        if (moveLeft) {
                            location.X += moveDelta;
                        }
                        else {
                            location.X -= moveDelta;
                        }
                        if (location.X < 0 || location.X + width > moveDetails.ContainerBounds.Right) {
                            // Overshot destination; put the rest back; move on.
                            if (location.X < 0) {
                                remainingDelta.Height = (float)location.X;
                                location.X = 0;
                            }
                            else {
                                remainingDelta.Height = (float)moveDetails.ContainerBounds.Right - width - (float)location.X;
                                location.X = moveDetails.ContainerBounds.Right - width;
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
                        if (location.Y > moveDetails.ContainerBounds.Height) {
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

            var squares = Enumerable.Range(0, squareCount).Select(boxIndex => {
                return new MovableBoxView(doSomeFunMoving) {
                    WidthRequest = 35,
                    HeightRequest = 35,
                    Color = ColorHelpers.GetRandomColor(rand),
                };
            }).ToList();
            funkyAbsoluteLayout = new AbsoluteLayout();
            foreach (var square in squares) {
                funkyAbsoluteLayout.Children.Add(square);
            }
            ;
            const double rowSpacing = 0;
            const double columnSpacing = 0;
            funkyAbsoluteLayout.SizeChanged += (sender, e) => {
                // Lay out a bunch of things manually, because YOLO and such.

                // NOTE: assumes all items identical in this version.
                var itemWidth = squares.First().Width;
                var itemsPerRow = (int)Math.Floor(funkyAbsoluteLayout.Bounds.Width / itemWidth);
                var remainderSpace = funkyAbsoluteLayout.Bounds.Width - (itemsPerRow * itemWidth);
                var xMargin = remainderSpace / 2;
                double x = xMargin;
                double y = 0;
                foreach (var square in squares) {
                    if (x + square.Bounds.Width > funkyAbsoluteLayout.Width) {
                        // NOTE: totally fails to wrap words longer than `funkyAbsoluteLayout.Width`.
                        y += square.Bounds.Height + rowSpacing;
                        x = xMargin;
                    }
                    var newBounds = new Rectangle(new Point(x, y), square.Bounds.Size);
                    AbsoluteLayout.SetLayoutBounds(square, newBounds);
                    x = newBounds.Right + columnSpacing;
                }
            };

            var touchesContentView = new TouchesContentView {
                Content = funkyAbsoluteLayout,
                IsClippedToBounds = true,
            };
            var contentPage = new ContentPage {
                Content = touchesContentView,
                Padding = new Thickness(5, Device.OnPlatform(iOS: 20, Android: 0, WinPhone: 0), 5, 5),
            };
            touchesContentView.OnPanned += (object sender, SizeF e) => {
                // TODO: Parallel.ForEach?
                var containerSize = funkyAbsoluteLayout.Bounds;
                foreach (var square in squares) {
                    var moveDetails = new MoveDetails {
                        ContainerBounds = containerSize,
                        CurrentBounds = square.Bounds,
                        DeltaMovement = e,
                    };
                    var resultMoveDetails = square.CalculateNewPosition(moveDetails);
                    AbsoluteLayout.SetLayoutBounds(square, resultMoveDetails.CurrentBounds);
                }
            };
            MainPage = contentPage;
        }
    }
}
