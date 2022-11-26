using System;

namespace BetterZoom.Edits;

public class ILEditException : Exception
{
	public ILEditException(string source) 
		: base($"IL edit at {source} failed! Please contact NotLe0n!")
	{ }
}