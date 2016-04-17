using InputHelper;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TouchScreenBuddy
{
	/// <summary>
	/// Interface for the touch manager item.
	/// </summary>
	public interface ITouchManager : IInputHelper
	{
		/// <summary>
		/// All the points that have new taps, stored in game coordinates.
		/// </summary>
		List<Vector2> Taps { get; }

		/// <summary>
		/// All the points that are currently being touched, in game coordinates.
		/// </summary>
		List<Vector2> Touches { get; }

		/// <summary>
		/// Check if touch is even enabled. will be false if no touch screen available.
		/// </summary>
		bool IsEnabled { get; }
	}
}
