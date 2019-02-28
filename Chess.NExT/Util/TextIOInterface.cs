using System;

namespace Chess.Utility
{
	public interface TextIOInterface
	{
		String ReadLine();
		
		void Write<T>(T output);
		void WriteLine<T>(T output);
	}
}