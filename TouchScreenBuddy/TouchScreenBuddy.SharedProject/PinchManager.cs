﻿using Microsoft.Xna.Framework;

namespace TouchScreenBuddy
{
	public class PinchManager
	{
		public Vector2 First { get; set; }
		public Vector2 Second { get; set; }

		public float Delta { get; set; }

		public bool Finished { get; set; }

		public PinchManager()
		{
			First = Vector2.Zero;
			Second = Vector2.Zero;
			Delta = 0f;
			Finished = false;
		}

		public PinchManager(Vector2 first, Vector2 second)
		{
			First = first;
			Second = second;
			Delta = 0f;
			Finished = false;
		}

		public void Update(Vector2 first, Vector2 second)
		{
			//First get the current distance between 1 & 2
			var distance1 = (Second - First).Length();

			//get the distance betweeen the second set of points
			var distance2 = (second - first).Length();

			//set the delta as the change between them
			Delta = (distance2 - distance1);

			//update the stored points
			First = first;
			Second = second;
		}
	}
}
