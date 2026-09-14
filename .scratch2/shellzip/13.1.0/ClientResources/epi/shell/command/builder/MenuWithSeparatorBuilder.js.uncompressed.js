define("epi/shell/command/builder/MenuWithSeparatorBuilder", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/on",
    "dijit/MenuSeparator",
    "./MenuBuilder"
], function (array, declare, lang, on, MenuSeparator, MenuBuilder) {
    return declare([MenuBuilder], {
        // summary:
        //      Builds a context menu with separator.
        //
        // tags:
        //      internal
        _addToContainer: function (widget, container) {
            // summary:
            //		Adds the widget to the container.
            // tags:
            //		protected
            this.inherited(arguments, [widget, container]);
            if (widget._commandCategory === "menuWithSeparator") {
                // Add the separator after menu item
                var separator = new MenuSeparator({ _command: { order: widget._command.order } });
                this.inherited(arguments, [separator, container]);
                // For available command but not the spliter one
                // then hides the separator when it is unavailable and listen to the change of the command's availability
                // to shows again.
                if (widget._command && !widget._command.isSplitter) {
                    separator.domNode.style.display = widget._command.get("isAvailable") ? "" : "none";
                    separator.own(separator.watch(widget._command, "isAvailable", function (name, oldValue, isAvailable) {
                        separator.domNode.style.display = isAvailable ? "" : "none";
                    }));
                }
            }
        },
        _onBeforeChildDestroy: function (container, index) {
            var children = this._getChildren(container);
            if (index < children.length - 1) {
                // separator is placed after menu item. It should also be removed
                var separatorChild = children[index + 1];
                if (container && container.declaredClass) {
                    container.removeChild(separatorChild);
                }
                separatorChild.destroy();
            }
        }
    });
});
