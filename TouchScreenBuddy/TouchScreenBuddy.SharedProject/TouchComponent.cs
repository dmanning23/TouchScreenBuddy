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