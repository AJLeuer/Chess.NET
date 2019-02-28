using System;

namespace Chess.Utility
{
	public class ConsoleTextIOInterface : TextIOInterface
	{
		public String ReadLine()
		{
			return Console.ReadLine();
		}

		public void Write<T>(T output)
		{
			Console.Write((dynamic) output);
		}

		public void WriteLine<T>(T output)
		{
			Console.WriteLine((dynamic) output);
		}
	}
}