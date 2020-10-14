namespace ConsoleGameEngineCore {
	/// <summary> Utility class. </summary>
	public class Utility {
		static public int Clamp(int a, int min, int max) {
			a = (a > max) ? max : a;
			a = (a < min) ? min : a;

			return a;
		}
	}
}
