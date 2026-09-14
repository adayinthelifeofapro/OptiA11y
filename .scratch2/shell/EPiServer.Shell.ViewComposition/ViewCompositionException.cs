using System;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Exception thrown when the application fails to assemble an <see cref="T:EPiServer.Shell.ViewComposition.ICompositeView" />.
///       </summary>
public class ViewCompositionException : Exception
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ViewCompositionException" /> class.
	///       </summary>
	public ViewCompositionException()
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ViewCompositionException" /> class.
	///       </summary>
	/// <param name="message">The error message explaining the reason for the exception.</param>
	public ViewCompositionException(string message)
		: base(message)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ViewCompositionException" /> class.
	///       </summary>
	/// <param name="message">The error message explaining the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of this exception, or null.</param>
	public ViewCompositionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
