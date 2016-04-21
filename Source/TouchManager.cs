using InputHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace TouchScreenBuddy
{
	/// <summary>
	/// delegate to convert a screen coordinate to a game coordinate
	/// </summary>
	/// <param name="screenCoord"></param>
	/// <returns></returns>
	public delegate Vector2 ConvertToGameCoord(Vector2 screenCoord);

	/// <summary>
	/// This thing is a central location to check for any taps or touches on a touchscreen device.
	/// </summary>
	public class TouchManager : ITouchManager
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
		/// All the points that have new taps, stored in game coordinates.
		/// </summary>
		public List<Vector2> Taps { get; private set; }

		/// <summary>
		/// All the points that are currently being touched, in game coordinates.
		/// </summary>
		public List<Vector2> Touches { get; private set; }

		/// <summary>
		/// Whether or not touch is enabled
		/// </summary>
		public bool IsEnabled { get; private set; }

		/// <summary>
		/// method used to convert coordinates
		/// </summary>
		private ConvertToGameCoord _gameCoord;

		private TouchLocation[] TouchStartPosition { get; set; }

		public List<ClickEventArgs> Clicks
		{
			get; private set;
		}

		public List<HighlightEventArgs> Highlights
		{
			get; private set;
		}

		public List<DragEventArgs> Drags
		{
			get; private set;
		}

		public List<DropEventArgs> Drops
		{
			get; private set;
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor
		/// </summary>
		public TouchManager(ConvertToGameCoord gameCoord = null)
		{
			Taps = new List<Vector2>();
			Touches = new List<Vector2>();

			Clicks = new List<ClickEventArgs>();
			Highlights = new List<HighlightEventArgs>();
			Drags = new List<DragEventArgs>();
			Drops = new List<DropEventArgs>();

			TouchStartPosition = new TouchLocation[_numTouches];

			_gameCoord = gameCoord;

			//Check if we even have a touchscreen available
			TouchPanelCapabilities touch = TouchPanel.GetCapabilities();
			IsEnabled = touch.IsConnected;

			//enable the tap gesture if available
			if (IsEnabled)
			{
				TouchPanel.EnabledGestures = GestureType.Tap;
			}
		}

		/// <summary>
		/// Update the mouse manager.
		/// </summary>
		public void Update(bool isActive)
		{
			//clear out the taps & touches
			Taps.Clear();
			Touches.Clear();
			Clicks.Clear();
			Highlights.Clear();
			Drags.Clear();
			Drops.Clear();

			if (isActive)
			{
				//get the new taps & touches
				GetTaps();
				GetTouches();

				//check if the player is holding down on the screen
				foreach (var touch in Touches)
				{
					Highlights.Add(new HighlightEventArgs()
					{
						Position = touch
					});
				}

				//check if the player tapped on the screen
				foreach (var tap in Taps)
				{
					Clicks.Add(new ClickEventArgs()
					{
						Position = tap,
						Button = MouseButton.Left
					});
				}
			}
		}

		/// <summary>
		/// get all the current tap gestures
		/// </summary>
		private void GetTaps()
		{
			//go through the taps and get all the new ones
			while (TouchPanel.IsGestureAvailable)
			{
				GestureSample gesture = TouchPanel.ReadGesture();
				if (gesture.GestureType == GestureType.Tap)
				{
					Taps.Add(ConvertCoordinate(gesture.Position));
				}
			}
		}

		/// <summary>
		/// get all the current touches
		/// </summary>
		private void GetTouches()
		{
			//go though the points that are being touched
			TouchCollection touchCollection = TouchPanel.GetState();
			foreach (var touch in touchCollection)
			{
				var touchIndex = touch.Id % _numTouches;

				if (touch.State == TouchLocationState.Pressed)
				{
					Touches.Add(ConvertCoordinate(touch.Position));

					//set the drag operation, just in case 
					TouchStartPosition[touchIndex] = touch;
				}
				else if (touch.State == TouchLocationState.Moved)
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

		/// <summary>
		/// Convert a coordinate from screen to game
		/// </summary>
		/// <param name="screenCoord"></param>
		/// <returns></returns>
		private Vector2 ConvertCoordinate(Vector2 screenCoord)
		{
			//use the delegate is available
			return ((null != _gameCoord) ? _gameCoord(screenCoord) : screenCoord);
		}
	
		#endregion //Methods
	}
}
