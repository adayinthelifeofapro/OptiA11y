using System;

namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create date/time selector.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(DateTime?))]
public class NullableDateTimeEditorDescriptor : EditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.NullableDateTimeEditorDescriptor" /> class.
	///       </summary>
	public NullableDateTimeEditorDescriptor()
	{
		base.ClientEditingClass = "epi/shell/widget/NullableDateTimeSelectorDropDown";
	}
}
