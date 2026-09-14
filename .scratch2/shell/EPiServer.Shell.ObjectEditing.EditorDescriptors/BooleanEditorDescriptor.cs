using System;
using System.Collections.Generic;

namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a checkbox to be able to edit boolean values.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(bool))]
[EditorDescriptorRegistration(TargetType = typeof(bool?))]
public class BooleanEditorDescriptor : EditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.BooleanEditorDescriptor" /> class.
	///       </summary>
	public BooleanEditorDescriptor()
	{
		base.ClientEditingClass = "epi/shell/widget/CheckBox";
	}

	public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
	{
		base.ModifyMetadata(metadata, attributes);
		metadata.EditorConfiguration["inlineEditor"] = true;
	}
}
