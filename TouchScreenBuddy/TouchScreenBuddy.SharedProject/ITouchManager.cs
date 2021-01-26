using InputHelper;
using Microsoft.Xna.Framework.Input.Touch;

namespace TouchScreenBuddy
{
	/// <summary>
	/// Interface for the touch manager item.
	/// </summary>
	public interface ITouchManager : IInputHelper
	{
		/// <summary>
		/// Check if touch is even enabled. will be false if no touch screen available.
		/// </summary>
		bool IsEnabled { get; set; }

		GestureType SupportedGestures { get; set; }

		/// <summary>
		/// The minimum length of a gesture for it to register as a flick.
		/// Defaults to 8000, make it shorter for 1080p?
		/// </summary>
		float FlickMinLength { get; set; }
	}
}
