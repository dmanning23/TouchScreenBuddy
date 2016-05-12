using InputHelper;

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
		bool IsEnabled { get; }
	}
}
