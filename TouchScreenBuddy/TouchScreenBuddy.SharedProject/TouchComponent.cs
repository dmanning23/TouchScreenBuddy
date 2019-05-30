using InputHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace TouchScreenBuddy
{
	/// <summary>
	/// MonoGame componenet that manages some simple Touch state junk.
	/// </summary>
	public class TouchComponent : GameComponent, ITouchManager
	{
		#region Properties

		public TouchManager TouchManager
		{
			get; private set;
		}

		public List<ClickEventArgs> Clicks
		{
			get
			{
				return TouchManager.Clicks;
			}
		}

		public List<HighlightEventArgs> Highlights
		{
			get
			{
				return TouchManager.Highlights;
			}
		}

		public List<DragEventArgs> Drags
		{
			get
			{
				return TouchManager.Drags;
			}
		}

		public List<DropEventArgs> Drops
		{
			get
			{
				return TouchManager.Drops;
			}
		}

		public List<FlickEventArgs> Flicks
		{
			get
			{
				return TouchManager.Flicks;
			}
		}

		public List<PinchEventArgs> Pinches
		{
			get
			{
				return TouchManager.Pinches;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return TouchManager.IsEnabled;
			}
		}

		public GestureType SupportedGestures
		{
			get
			{
				return TouchManager.SupportedGestures;
			}
			set
			{
				TouchManager.SupportedGestures = value;
			}
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructs a new input state.
		/// </summary>
		public TouchComponent(Game game, ConvertToGameCoord gameCoord)
			: base(game)
		{
			TouchManager = new TouchManager(gameCoord);

			//Register ourselves to implement the DI container service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IInputHelper), this);
		}
		
		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Reads the latest state of the keyboard and gamepad.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			TouchManager.Update(Game.IsActive);
		}

		#endregion //Methods
	}
}