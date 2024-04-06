using System;

namespace BetterZoom.Edits;

public sealed class ILEditException : Exception
{
	public ILEditException(string source) 
		: base($"IL edit at BetterZoom.{source} failed! Please contact NotLe0n!")
	{ }
}