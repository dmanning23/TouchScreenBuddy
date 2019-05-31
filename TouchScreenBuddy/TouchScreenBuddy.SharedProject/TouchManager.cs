using InputHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace TouchScreenBuddy
{
	/// <summary>
	/// This thing is a central location to check for any taps or touches on a touchscreen device.
	/// </summary>
	public class TouchManager : BaseInputManager, ITouchManager
	{
		#region Fields

		/// <summary>
		/// pixels^2 from start position to register as a drag and not a click
		/// </summary>
		private const float _dragDelta = 25f;

		private const int _numTouches = 10;

		#endregion //Fields

		#region Properties

		/// <summary>
		/// Whether or not touch is enabled
		/// </summary>
		public bool IsEnabled { get; private set; }

		private TouchLocation[] TouchStartPosition { get; set; }

		public GestureType SupportedGestures
		{
			get
			{
				return IsEnabled ? TouchPanel.EnabledGestures : GestureType.None;
			}
			set
			{
				if (IsEnabled)
				{
					TouchPanel.EnabledGestures = value;
				}
			}
		}

		TouchCollection TouchCollection { get; set; }

		PinchManager Pinch;

		public float FlickMinLength { get; set; } = 8000f;

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor
		/// </summary>
		public TouchManager(ConvertToGameCoord gameCoord, GestureType supportedGestures = GestureType.Tap) : base(gameCoord)
		{
			TouchStartPosition = new TouchLocation[_numTouches];

			//Check if we even have a touchscreen available
			TouchPanelCapabilities touch = TouchPanel.GetCapabilities();
			IsEnabled = touch.IsConnected;

			//enable the tap gesture if available
			SupportedGestures = supportedGestures;
		}

		/// <summary>
		/// Update the mouse manager.
		/// </summary>
		public override void Update(bool isActive)
		{
			//clear out the taps & touches
			Clicks.Clear();
			Highlights.Clear();
			Drags.Clear();
			Drops.Clear();
			Flicks.Clear();
			Pinches.Clear();

			if (null != Pinch)
			{
				//reset the pinch delta
				Pinch.Delta = 0f;
			}

			if (isActive)
			{
				TouchCollection = TouchPanel.GetState();

				//get the new taps & touches
				GetGestures();
				GetTouches();
			}

			//Add the pinch event if there is an ongoing gesture
			if (null != Pinch)
			{
				Pinches.Add(new PinchEventArgs(Pinch.Delta));
			}
		}

		/// <summary>
		/// get all the current tap gestures
		/// </summary>
		private void GetGestures()
		{
			//go through the taps and get all the new ones
			while (TouchPanel.IsGestureAvailable)
			{
				GestureSample gesture = TouchPanel.ReadGesture();
				switch (gesture.GestureType)
				{
					case GestureType.Tap:
						{
							var position = ConvertCoordinate(gesture.Position);
							Clicks.Add(new ClickEventArgs()
							{
								Position = position,
								Button = MouseButton.Left
							});
						}
						break;

					case GestureType.DoubleTap:
						{
							var position = ConvertCoordinate(gesture.Position);
							Clicks.Add(new ClickEventArgs()
							{
								Position = position,
								Button = MouseButton.Left,
								DoubleClick = true,
							});
						}
						break;

					case GestureType.Flick:
						{
							AddFlickEvent(gesture.Delta);
						}
						break;

					case GestureType.Pinch:
						{
							var position1 = ConvertCoordinate(gesture.Position);
							var position2 = ConvertCoordinate(gesture.Position2);

							if (null == Pinch)
							{
								Pinch = new PinchManager(position1, position2);
							}
							else
							{
								Pinch.Update(position1, position2);
							}
						}
						break;

					case GestureType.PinchComplete:
						{
							Pinch = null;
						}
						break;
				}
			}
		}

		private void AddFlickEvent(Vector2 delta)
		{
			//Was that gesture strong enough to register as a "flick"?
			var deltaLength = delta.Length();
			if (deltaLength < FlickMinLength)
			{
				return;
			}

			//go though the points that are being touched
			foreach (var touch in TouchCollection)
			{
				var touchIndex = touch.Id % _numTouches;

				//Sometimes TryGetPreviousLocation can fail. 
				//Bail out early if this happened or if the last state didn't move
				TouchLocation prevLoc;
				if (!touch.TryGetPreviousLocation(out prevLoc) ||
					TouchStartPosition[touchIndex].Id != touch.Id)
				{
					continue;
				}

				//get the end of the event
				Flicks.Add(new FlickEventArgs()
				{
					Position = ConvertCoordinate(touch.Position),
					Delta = delta,
				});
			}
		}

		/// <summary>
		/// get all the current touches
		/// </summary>
		private void GetTouches()
		{
			//go though the points that are being touched
			foreach (var touch in TouchCollection)
			{
				var touchIndex = touch.Id % _numTouches;

				if (touch.State == TouchLocationState.Pressed)
				{
					AddHighlightEvent(touch);

					//set the drag operation, just in case 
					TouchStartPosition[touchIndex] = touch;
				}
				else if (touch.State == TouchLocationState.Moved)
				{
					AddHighlightEvent(touch);

					//Sometimes TryGetPreviousLocation can fail. 
					//Bail out early if this happened or if the last state didn't move
					TouchLocation prevLoc;
					if (!touch.TryGetPreviousLocation(out prevLoc) ||
						prevLoc.State != TouchLocationState.Moved ||
						TouchStartPosition[touchIndex].Id != touch.Id)
					{
						continue;
					}

					// get your delta
					var delta = touch.Position - prevLoc.Position;

					//fire off drag event
					Drags.Add(new DragEventArgs()
					{
						Start = ConvertCoordinate(TouchStartPosition[touchIndex].Position),
						Current = ConvertCoordinate(touch.Position),
						Delta = (touch.Position - prevLoc.Position),
						Button = MouseButton.Left
					});
				}
				else if (touch.State == TouchLocationState.Released)
				{
					//Sometimes TryGetPreviousLocation can fail. 
					//Bail out early if this happened or if the last state didn't move
					TouchLocation prevLoc;
					if (!touch.TryGetPreviousLocation(out prevLoc) ||
						prevLoc.State != TouchLocationState.Moved ||
						TouchStartPosition[touchIndex].Id != touch.Id)
					{
						continue;
					}

					// get your delta
					var dragStartPosition = ConvertCoordinate(TouchStartPosition[touchIndex].Position);
					var touchPos = ConvertCoordinate(touch.Position);
					var delta = touchPos - dragStartPosition;

					// Usually you don't want to do something if the user drags 1 pixel.
					if (delta.LengthSquared() < _dragDelta)
					{
						continue;
					}

					//fire off drop event
					Drops.Add(new DropEventArgs()
					{
						Start = dragStartPosition,
						Drop = touchPos,
						Button = MouseButton.Left
					});
				}
			}
		}

		private void AddHighlightEvent(TouchLocation touch)
		{
			//add a highlight event at that location
			Highlights.Add(new HighlightEventArgs(ConvertCoordinate(touch.Position), this));
		}

		#endregion //Methods
	}
}
