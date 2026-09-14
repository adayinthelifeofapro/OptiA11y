using System;

namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create date/time selector.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(DateTime))]
public class DateTimeEditorDescriptor : EditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.DateTimeEditorDescriptor" /> class.
	///       </summary>
	public DateTimeEditorDescriptor()
	{
		base.ClientEditingClass = "epi/shell/widget/DateTimeSelectorDropDown";
	}
}
