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

		public List<ClickEventArgs> Clicks => TouchManager.Clicks;

		public List<HighlightEventArgs> Highlights => TouchManager.Highlights;

		public List<DragEventArgs> Drags => TouchManager.Drags;

		public List<DropEventArgs> Drops => TouchManager.Drops;

		public List<FlickEventArgs> Flicks => TouchManager.Flicks;

		public List<PinchEventArgs> Pinches => TouchManager.Pinches;

		public List<HoldEventArgs> Holds => TouchManager.Holds;

		public bool IsEnabled
		{
			get
			{
				return TouchManager.IsEnabled;
			}
			set
			{
				TouchManager.IsEnabled = value;
			}
		}

		public GestureType SupportedGestures
		{
			get => TouchManager.SupportedGestures;
			set => TouchManager.SupportedGestures = value;
		}

		public float FlickMinLength => TouchManager.FlickMinLength;

		float ITouchManager.FlickMinLength
		{
			get => TouchManager.FlickMinLength;
			set => TouchManager.FlickMinLength = value;
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
			game.Services.AddService(typeof(ITouchManager), this);
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