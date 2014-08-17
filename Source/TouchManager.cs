using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using ResolutionBuddy;
using System.Collections.Generic;

namespace TouchScreenBuddy
{
	/// <summary>
	/// This thing is a central location to check for any taps or touches on a touchscreen device.
	/// </summary>
	public class TouchManager : GameComponent, ITouchManager
	{
		#region Properties

		/// <summary>
		/// All the points that have new taps, stored in game coordinates.
		/// </summary>
		public List<Vector2> Taps { get; private set; }

		/// <summary>
		/// All the points that are currently being touched, in game coordinates.
		/// </summary>
		public List<Vector2> Touches { get; private set; }

		public bool IsEnabled { get; private set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="game"></param>
		public TouchManager(Game game)
			: base(game)
		{
			Taps = new List<Vector2>();
			Touches = new List<Vector2>();

			//Check if we even have a touchscreen available
			TouchPanelCapabilities touch = TouchPanel.GetCapabilities();
			IsEnabled = touch.IsConnected;

			//enable the tap gesture if available
			if (IsEnabled)
			{
				TouchPanel.EnabledGestures = GestureType.Tap;
			}

			// Register ourselves to implement the IMessageDisplay service.
			game.Services.AddService(typeof(ITouchManager), this);
		}

		/// <summary>
		/// update method, called every frame to get the taps & touches
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (IsEnabled)
			{
				//clear out the taps & touches
				Taps.Clear();
				Touches.Clear();

				//get the new taps & touches
				GetTaps();
				GetTouches();
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
					Taps.Add(Resolution.ScreenToGameCoord(gesture.Position));
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
				if ((touch.State == TouchLocationState.Pressed) || (touch.State == TouchLocationState.Moved))
				{
					Touches.Add(Resolution.ScreenToGameCoord(touch.Position));
				}
			}
		}
	
		#endregion //Methods
	}
}
