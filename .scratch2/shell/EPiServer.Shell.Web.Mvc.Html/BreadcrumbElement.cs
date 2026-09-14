using EPiServer.Core;

namespace EPiServer.Shell.Web.Mvc.Html;

public class BreadcrumbElement
{
	public ContentReference Id { get; set; }

	public string Name { get; set; }

	public BreadcrumbElement()
	{
	}

	public BreadcrumbElement(ContentReference id, string name)
	{
		Id = id;
		Name = name;
	}
}
