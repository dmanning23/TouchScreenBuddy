using InputHelper;
using Microsoft.Xna.Framework;
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

		public List<Vector2> Taps
		{
			get
			{
				return TouchManager.Taps;
			}
		}

		public List<Vector2> Touches
		{
			get
			{
				return TouchManager.Touches;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return TouchManager.IsEnabled;
			}
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructs a new input state.
		/// </summary>
		public TouchComponent(Game game)
			: base(game)
		{
			TouchManager = new TouchManager();

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