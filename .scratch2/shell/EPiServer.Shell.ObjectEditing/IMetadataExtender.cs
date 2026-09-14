using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Implement this interface to be able to modify metadata.
///       </summary>
public interface IMetadataExtender : IMetadataHandler
{
	/// <summary>
	///       Use to add change metadata for the editor before the UI is rendered.
	///       </summary>
	/// <param name="metadata">The metadata.</param>
	/// <param name="attributes">The attributes.</param>
	void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
	{
	}

	/// <summary>
	///       Asynchronous version of <see cref="M:EPiServer.Shell.ObjectEditing.IMetadataExtender.ModifyMetadata(EPiServer.Shell.ObjectEditing.ExtendedMetadata,System.Collections.Generic.IEnumerable{System.Attribute})" />.
	///       Override this method in new implementations that need async operations.
	///       </summary>
	/// <param name="metadata">The metadata.</param>
	/// <param name="attributes">The attributes.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task ModifyMetadataAsync(ExtendedMetadata metadata, IEnumerable<Attribute> attributes, CancellationToken cancellationToken = default(CancellationToken))
	{
		ModifyMetadata(metadata, attributes);
		return Task.CompletedTask;
	}
}
