using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Html helpers form input tags
///       </summary>
public static class InputHelperExtensions
{
	private static int _idCounter;

	/// <summary>
	///       Creates a select list for usage in a drop down list.
	///       </summary>
	/// <param name="html">The object which this method extends</param>
	/// <param name="selectedValue">The selected value in the select list</param>
	/// <param name="values">The values of the select list</param>
	/// <returns>A loaded list of select list items</returns>
	public static IEnumerable<SelectListItem> SelectList(this IHtmlHelper html, object selectedValue, params object[] values)
	{
		return new SelectList(values, selectedValue);
	}

	/// <summary>
	///       Creates a select list for usage in a drop down list.
	///       </summary>
	/// <param name="html">The object which this method extends</param>
	/// <param name="selectedValue">The selected value in the select list</param>
	/// <param name="dataValueField">Name of the field used as the value attribute.</param>
	/// <param name="dataTextField">Name of the field used as the text value.</param>
	/// <param name="values">The values of the select list</param>
	/// <returns>A loaded list of select list items</returns>
	public static IEnumerable<SelectListItem> SelectList(this IHtmlHelper html, object selectedValue, string dataValueField, string dataTextField, object[] values)
	{
		return new SelectList(values, dataValueField, dataTextField, selectedValue);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="inputType">Type of the input.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellButton(this IHtmlHelper html, ShellInputType inputType, string name, string value, string cssClass, object htmlAttributes)
	{
		if (inputType != ShellInputType.Button && inputType != ShellInputType.Submit && inputType != ShellInputType.Reset)
		{
			throw new ArgumentException("InputType needs to be button, submit or reset i.e. a button");
		}
		return html.ShellInput(inputType, name, value, cssClass, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellButton(this IHtmlHelper html, string name, string value, object htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Button, name, value, null, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellButton(this IHtmlHelper html, string name, string value, IDictionary<string, object> htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Button, name, value, null, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellButton(this IHtmlHelper html, string name, string value, string cssClass, object htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Button, name, value, cssClass, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellButton(this IHtmlHelper html, string name, string value, string cssClass, IDictionary<string, object> htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Button, name, value, cssClass, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellResetButton(this IHtmlHelper html, string name, string value, object htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Reset, name, value, null, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellResetButton(this IHtmlHelper html, string name, string value, IDictionary<string, object> htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Reset, name, value, null, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellResetButton(this IHtmlHelper html, string name, string value, string cssClass, object htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Reset, name, value, cssClass, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellResetButton(this IHtmlHelper html, string name, string value, string cssClass, IDictionary<string, object> htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Reset, name, value, cssClass, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellSubmitButton(this IHtmlHelper html, string name, string value, object htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Submit, name, value, null, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellSubmitButton(this IHtmlHelper html, string name, string value, IDictionary<string, object> htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Submit, name, value, null, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellSubmitButton(this IHtmlHelper html, string name, string value, string cssClass, object htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Submit, name, value, cssClass, htmlAttributes);
	}

	/// <summary>
	///       Creates an input button of submit type with the appropriate styles.
	///       To be used when an EPiServer styled button is desired.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellSubmitButton(this IHtmlHelper html, string name, string value, string cssClass, IDictionary<string, object> htmlAttributes)
	{
		return html.ShellInput(ShellInputType.Submit, name, value, cssClass, htmlAttributes);
	}

	/// <summary>
	///       Creates an input with the appropriate styles.
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="inputType">Type of the input.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellInput(this IHtmlHelper html, ShellInputType inputType, string name, string value, object htmlAttributes)
	{
		return html.ShellInput(inputType, name, value, null, new RouteValueDictionary(htmlAttributes));
	}

	/// <summary>
	///       Creates an input with the appropriate styles.
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="inputType">Type of the input.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellInput(this IHtmlHelper html, ShellInputType inputType, string name, string value, string cssClass, object htmlAttributes)
	{
		return html.ShellInput(inputType, name, value, cssClass, new RouteValueDictionary(htmlAttributes));
	}

	/// <summary>
	///       Creates an input with the appropriate styles.
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="inputType">Type of the input.</param>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellInput(this IHtmlHelper html, ShellInputType inputType, string name, string value, IDictionary<string, object> htmlAttributes)
	{
		return html.ShellInput(inputType, name, value, null, new RouteValueDictionary(htmlAttributes));
	}

	/// <summary>
	///       Creates an input with the appropriate styles.
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="name">The name.</param>
	/// <param name="inputType">Type of the input.</param>
	/// <param name="value">The value.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent ShellInput(this IHtmlHelper html, ShellInputType inputType, string name, string value, string cssClass, IDictionary<string, object> htmlAttributes)
	{
		TagBuilder tagBuilder = new TagBuilder("input");
		tagBuilder.Attributes["type"] = inputType.ToString().ToLower(CultureInfo.InvariantCulture);
		if (name != null)
		{
			tagBuilder.Attributes["name"] = name;
		}
		if (value != null)
		{
			tagBuilder.Attributes["value"] = value;
		}
		tagBuilder.MergeAttributes(htmlAttributes);
		if ((inputType == ShellInputType.Button || (uint)(inputType - 7) <= 1u) ? true : false)
		{
			return html.AddShellButtonTags(tagBuilder, cssClass);
		}
		if (!string.IsNullOrEmpty(cssClass))
		{
			tagBuilder.AddCssClass(cssClass);
		}
		return tagBuilder;
	}

	/// <summary>
	///       Returns a checkbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The text of the label text.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent LabeledCheckBox(this IHtmlHelper htmlHelper, string name, string labelText)
	{
		return htmlHelper.LabeledCheckBox(name, labelText, isChecked: false);
	}

	/// <summary>
	///       Returns a checkbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The text of the label text.</param>
	/// <param name="isChecked">Set to <c>true</c> if the checkbox should be checked.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent LabeledCheckBox(this IHtmlHelper htmlHelper, string name, string labelText, bool isChecked)
	{
		return htmlHelper.LabeledCheckBox(name, labelText, isChecked, null, null);
	}

	/// <summary>
	///       Returns a checkbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The text of the label text.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The label HTML attributes.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent LabeledCheckBox(this IHtmlHelper htmlHelper, string name, string labelText, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledCheckBox(name, labelText, isChecked: false, htmlAttributes, labelHtmlAttributes);
	}

	/// <summary>
	///       Returns a checkbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The text of the label text.</param>
	/// <param name="isChecked">Set to <c>true</c> if the checkbox should be checked.</param>
	/// <param name="htmlAttributes">The HTML attributes for the input.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>An html string representing the input.</returns>
	public static IHtmlContent LabeledCheckBox(this IHtmlHelper htmlHelper, string name, string labelText, bool isChecked, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledCheckBox(name, labelText, isChecked, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a checkbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The text of the label text.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>An html string representing a checkbox with an associated label.</returns>
	public static IHtmlContent LabeledCheckBox(this IHtmlHelper htmlHelper, string name, string labelText, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		return htmlHelper.LabeledCheckBox(name, labelText, isChecked: false, htmlAttributes, labelHtmlAttributes);
	}

	/// <summary>
	///       Returns a checkbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The text of the label text.</param>
	/// <param name="isChecked">Set to <c>true</c> if the checkbox should be checked.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>An html string representing a checkbox with an associated label.</returns>
	public static IHtmlContent LabeledCheckBox(this IHtmlHelper htmlHelper, string name, string labelText, bool isChecked, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		string id = htmlHelper.GetId();
		if (htmlAttributes == null)
		{
			htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}
		htmlAttributes["id"] = id;
		IHtmlContent htmlContent = htmlHelper.CheckBox(name, isChecked, htmlAttributes);
		IHtmlContent content = htmlHelper.LabelFor(labelText, id, labelHtmlAttributes);
		List<XElement> list = XDocument.Parse(string.Format(CultureInfo.CurrentUICulture, "<root>{0}</root>", htmlContent)).Descendants("input").ToList();
		if (list.Count == 2)
		{
			htmlContentBuilder.Append(list.First().ToString()).AppendHtml(content).Append(list.Last().ToString());
		}
		else
		{
			htmlContentBuilder.AppendHtml(htmlContent).AppendHtml(content);
		}
		return htmlContentBuilder;
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <returns>
	///       An html string representing a drop-down list with an associated label.
	///       </returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText)
	{
		return htmlHelper.LabeledDropDownList(name, labelText, null, null);
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="selectList">The list of values used to populate the drop-down.</param>
	/// <returns>
	///       An html string representing a drop-down list with an associated label.
	///       </returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText, IEnumerable<SelectListItem> selectList)
	{
		return htmlHelper.LabeledDropDownList(name, labelText, selectList, null);
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="optionLabel">Provides the text for a default empty valued option, if it is not null.</param>
	/// <returns>
	///       An html string representing a drop-down list with an associated label.
	///       </returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText, string optionLabel)
	{
		return htmlHelper.LabeledDropDownList(name, labelText, null, optionLabel);
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="selectList">The list of values used to populate the drop-down.</param>
	/// <param name="htmlAttributes">The HTML attributes for the drop-down.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>
	///       An html string representing a drop-down list with an associated label.
	///       </returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		return htmlHelper.LabeledDropDownList(name, labelText, selectList, null, htmlAttributes, labelHtmlAttributes);
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="selectList">The list of values used to populate the drop-down.</param>
	/// <param name="htmlAttributes">The HTML attributes for the drop-down.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>
	///       An html string representing a drop-down list with an associated label.
	///       </returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText, IEnumerable<SelectListItem> selectList, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledDropDownList(name, labelText, selectList, null, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="selectList">The list of values used to populate the drop-down.</param>
	/// <param name="optionLabel">Provides the text for a default empty valued option, if it is not null.</param>
	/// <returns>
	///       An html string representing a drop-down list with an associated label.
	///       </returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText, IEnumerable<SelectListItem> selectList, string optionLabel)
	{
		return htmlHelper.LabeledDropDownList(name, labelText, selectList, optionLabel, null, null);
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="selectList">The list of values used to populate the drop-down.</param>
	/// <param name="optionLabel">Provides the text for a default empty valued option, if it is not null.</param>
	/// <param name="htmlAttributes">The HTML attributes for the drop-down.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>An html string representing a drop-down list with an associated label.</returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledDropDownList(name, labelText, selectList, optionLabel, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a drop-down with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the drop-down.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="selectList">The list of values used to populate the drop-down.</param>
	/// <param name="optionLabel">Provides the text for a default empty valued option, if it is not null.</param>
	/// <param name="htmlAttributes">The HTML attributes for the drop-down.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>An html string representing a drop-down list with an associated label.</returns>
	public static IHtmlContent LabeledDropDownList(this IHtmlHelper htmlHelper, string name, string labelText, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		string id = htmlHelper.GetId();
		if (htmlAttributes == null)
		{
			htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}
		htmlAttributes["id"] = id;
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		htmlContentBuilder.AppendHtml(htmlHelper.LabelFor(labelText, id, labelHtmlAttributes)).AppendHtml(htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes));
		return htmlContentBuilder;
	}

	/// <summary>
	///       Returns a textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <returns>
	///       An html string representing a textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextBox(this IHtmlHelper htmlHelper, string name, string labelText)
	{
		return htmlHelper.LabeledTextBox(name, labelText, null);
	}

	/// <summary>
	///       Returns a textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <returns>
	///       An html string representing a textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextBox(this IHtmlHelper htmlHelper, string name, string labelText, object value)
	{
		return htmlHelper.LabeledTextBox(name, labelText, value, null, null);
	}

	/// <summary>
	///       Returns a textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The label HTML attributes.</param>
	/// <returns>
	///       An html string representing a textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextBox(this IHtmlHelper htmlHelper, string name, string labelText, object value, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledTextBox(name, labelText, value, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the checkbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>
	///       An html string representing a textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextBox(this IHtmlHelper htmlHelper, string name, string labelText, object value, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		string id = htmlHelper.GetId();
		if (htmlAttributes == null)
		{
			htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}
		htmlAttributes["id"] = id;
		htmlContentBuilder.AppendHtml(htmlHelper.LabelFor(labelText, id, labelHtmlAttributes)).AppendHtml(htmlHelper.TextBox(name, value, htmlAttributes));
		return htmlContentBuilder;
	}

	/// <summary>
	///       Returns a textarea with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name.</param>
	/// <param name="labelText">The label text.</param>
	/// <returns>
	///       An html string representing a textarea with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextArea(this IHtmlHelper htmlHelper, string name, string labelText)
	{
		return htmlHelper.LabeledTextArea(name, labelText, null);
	}

	/// <summary>
	///       Returns a textarea with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>
	///       An html string representing a textarea with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextArea(this IHtmlHelper htmlHelper, string name, string labelText, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		return htmlHelper.LabeledTextArea(name, labelText, null, htmlAttributes, labelHtmlAttributes);
	}

	/// <summary>
	///       Returns a textarea with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>
	///       An html string representing a textarea with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextArea(this IHtmlHelper htmlHelper, string name, string labelText, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledTextArea(name, labelText, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a textarea with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <returns>
	///       An html string representing a textarea with an associated label.
	///       </returns>
	public static IHtmlContent LabeledTextArea(this IHtmlHelper htmlHelper, string name, string labelText, string value)
	{
		return htmlHelper.LabeledTextArea(name, labelText, value, null, null);
	}

	/// <summary>
	///       Returns a textarea with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>An html string representing a textarea with an associated label.</returns>
	public static IHtmlContent LabeledTextArea(this IHtmlHelper htmlHelper, string name, string labelText, string value, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledTextArea(name, labelText, value, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a textarea with an associated label.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The HTML attributes for the label.</param>
	/// <returns>An html string representing a textarea with an associated label.</returns>
	public static IHtmlContent LabeledTextArea(this IHtmlHelper htmlHelper, string name, string labelText, string value, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		string id = htmlHelper.GetId();
		if (htmlAttributes == null)
		{
			htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}
		htmlAttributes["id"] = id;
		htmlContentBuilder.AppendHtml(htmlHelper.LabelFor(labelText, id, labelHtmlAttributes)).AppendHtml(htmlHelper.TextArea(name, value, 5, 30, htmlAttributes));
		return htmlContentBuilder;
	}

	/// <summary>
	///       Returns a password textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the password textbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <returns>
	///       An html string representing a  password textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledPassword(this IHtmlHelper htmlHelper, string name, string labelText)
	{
		return htmlHelper.LabeledPassword(name, labelText, null);
	}

	/// <summary>
	///       Returns a password textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the password textbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <returns>
	///       An html string representing a  password textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledPassword(this IHtmlHelper htmlHelper, string name, string labelText, object value)
	{
		return htmlHelper.LabeledPassword(name, labelText, value, null, null);
	}

	/// <summary>
	///       Returns a password textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the password textbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The label HTML attributes.</param>
	/// <returns>
	///       An html string representing a  password textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledPassword(this IHtmlHelper htmlHelper, string name, string labelText, object value, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledPassword(name, labelText, value, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a password textbox with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the password textbox.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The label HTML attributes.</param>
	/// <returns>
	///       An html string representing a  password textbox with an associated label.
	///       </returns>
	public static IHtmlContent LabeledPassword(this IHtmlHelper htmlHelper, string name, string labelText, object value, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		string id = htmlHelper.GetId();
		if (htmlAttributes == null)
		{
			htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}
		htmlAttributes["id"] = id;
		htmlContentBuilder.AppendHtml(htmlHelper.LabelFor(labelText, id, labelHtmlAttributes)).AppendHtml(htmlHelper.Password(name, value, htmlAttributes));
		return htmlContentBuilder;
	}

	/// <summary>
	///       Returns a radiobutton with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the radiobutton.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <returns>
	///       An html string representing a radiobutton with an associated label.
	///       </returns>
	public static IHtmlContent LabeledRadioButton(this IHtmlHelper htmlHelper, string name, string labelText, object value)
	{
		return htmlHelper.LabeledRadioButton(name, labelText, value, isChecked: false);
	}

	/// <summary>
	///       Returns a radiobutton with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the radiobutton.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="isChecked">Set to <c>true</c> if the radiobutton should be checked.</param>
	/// <returns>
	///       An html string representing a radiobutton with an associated label.
	///       </returns>
	public static IHtmlContent LabeledRadioButton(this IHtmlHelper htmlHelper, string name, string labelText, object value, bool isChecked)
	{
		return htmlHelper.LabeledRadioButton(name, labelText, value, isChecked, null, null);
	}

	/// <summary>
	///       Returns a radiobutton with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the radiobutton.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The label HTML attributes.</param>
	/// <returns>
	///       An html string representing a radiobutton with an associated label.
	///       </returns>
	public static IHtmlContent LabeledRadioButton(this IHtmlHelper htmlHelper, string name, string labelText, object value, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledRadioButton(name, labelText, value, isChecked: false, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a radiobutton with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the radiobutton.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="isChecked">Set to <c>true</c> if the radiobutton should be checked.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The label HTML attributes.</param>
	/// <returns>
	///       An html string representing a radiobutton with an associated label.
	///       </returns>
	public static IHtmlContent LabeledRadioButton(this IHtmlHelper htmlHelper, string name, string labelText, object value, bool isChecked, object htmlAttributes, object labelHtmlAttributes)
	{
		return htmlHelper.LabeledRadioButton(name, labelText, value, isChecked, new RouteValueDictionary(htmlAttributes), new RouteValueDictionary(labelHtmlAttributes));
	}

	/// <summary>
	///       Returns a radiobutton with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="name">The name of the radiobutton.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="value">The value.</param>
	/// <param name="isChecked">Set to <c>true</c> if the radiobutton should be checked.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="labelHtmlAttributes">The label HTML attributes.</param>
	/// <returns>
	///       An html string representing a radiobutton with an associated label.
	///       </returns>
	public static IHtmlContent LabeledRadioButton(this IHtmlHelper htmlHelper, string name, string labelText, object value, bool isChecked, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelHtmlAttributes)
	{
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		string id = htmlHelper.GetId();
		if (htmlAttributes == null)
		{
			htmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}
		htmlAttributes["id"] = id;
		htmlContentBuilder.AppendHtml(htmlHelper.RadioButton(name, value, isChecked, htmlAttributes)).AppendHtml(htmlHelper.LabelFor(labelText, id, labelHtmlAttributes));
		return htmlContentBuilder;
	}

	/// <summary>
	///       Returns a radiobutton with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="inputId">The input id.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>
	///       An html string representing a label.
	///       </returns>
	public static IHtmlContent LabelFor(this IHtmlHelper htmlHelper, string labelText, string inputId, object htmlAttributes)
	{
		return htmlHelper.Label(labelText, inputId, new RouteValueDictionary(htmlAttributes));
	}

	/// <summary>
	///       Returns a radiobutton with an associated label
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <param name="labelText">The label text.</param>
	/// <param name="inputId">The input id.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <returns>
	///       An html string representing a label.
	///       </returns>
	public static IHtmlContent LabelFor(this IHtmlHelper htmlHelper, string labelText, string inputId, IDictionary<string, object> htmlAttributes)
	{
		TagBuilder tagBuilder = new TagBuilder("label");
		tagBuilder.MergeAttributes(htmlAttributes);
		tagBuilder.Attr("for", inputId);
		tagBuilder.InnerHtml.Append(labelText);
		return tagBuilder;
	}

	/// <summary>
	///       Gets the next unique id.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper.</param>
	/// <returns>
	/// </returns>
	public static string GetId(this IHtmlHelper htmlHelper)
	{
		if (_idCounter == 99999)
		{
			_idCounter = 0;
		}
		return "id" + Interlocked.Increment(ref _idCounter);
	}

	internal static TagBuilder AddShellButtonTags(this IHtmlHelper htmlHelper, TagBuilder builder, string cssClass)
	{
		builder.AddCssClass("epi-button-child-item");
		TagBuilder tagBuilder = new TagBuilder("span");
		tagBuilder.Attributes["class"] = "epi-button-child";
		tagBuilder.InnerHtml.AppendHtml(builder);
		TagBuilder tagBuilder2 = new TagBuilder("span");
		tagBuilder2.AddCssClass("epi-button");
		if (!string.IsNullOrEmpty(cssClass))
		{
			tagBuilder2.AddCssClass(cssClass);
		}
		tagBuilder2.InnerHtml.AppendHtml(tagBuilder);
		return tagBuilder2;
	}
}
