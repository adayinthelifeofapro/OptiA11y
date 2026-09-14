using System;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Exception thrown when a configuration problems occurred.
///       </summary>
public class MissingConfigurationException : Exception
{
	/// <summary>Initializes <see cref="T:EPiServer.Shell.Configuration.MissingConfigurationException" /> instance.</summary>
	public MissingConfigurationException()
	{
	}

	/// <summary>Initializes <see cref="T:EPiServer.Shell.Configuration.MissingConfigurationException" /> instance.</summary>
	/// <param name="message">The message propagated to the Exception</param>
	public MissingConfigurationException(string message)
		: base(message)
	{
	}

	/// <summary>Initializes <see cref="T:EPiServer.Shell.Configuration.MissingConfigurationException" /> instance.</summary>
	/// <param name="message">The message propagated to the Exception</param>
	/// <param name="inner">The exception that caused this exception.</param>
	public MissingConfigurationException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
