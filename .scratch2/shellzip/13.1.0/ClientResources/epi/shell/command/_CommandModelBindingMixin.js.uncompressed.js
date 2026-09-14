define("epi/shell/command/_CommandModelBindingMixin", [
    "dojo/_base/declare",
    "dojo/dom-style",
    "dojo/dom-class",
    "../widget/_ModelBindingMixin"
], function (declare, domStyle, domClass, _ModelBindingMixin) {
    function showSeparatorEl(domNode, commandCategory, isAvailable, showSeparator) {
        if (!domNode) {
            return;
        }
        if (commandCategory !== "menuWithSeparator") {
            return;
        }
        var display = true;
        if (!isAvailable) {
            display = false;
        }
        else {
            showSeparator = showSeparator === undefined ? true : showSeparator;
            display = showSeparator;
        }
        domNode.nextSibling.style.display = display ? "" : "none";
    }
    return declare(_ModelBindingMixin, {
        // summary:
        //      A widget mixin for binding command properties to widget properties.
        //
        // tags:
        //      internal xproduct
        // viewModelBindingMap: [protected] Object
        //      Configuration of command property to widget property mappings.
        modelBindingMap: {
            label: ["label"],
            tooltip: ["tooltip"],
            iconClass: ["iconClass"],
            canExecute: ["canExecute"],
            isExecuting: ["isExecuting"],
            isAvailable: ["isAvailable"],
            active: ["checked", "isExpand"],
            showSeparator: ["showSeparator"]
        },
        _setCanExecuteAttr: function (/*Boolean*/ canExecute) {
            // summary:
            //      Sets the widget as enabled or disabled based on the value of canExecute
            // tags:
            //      private
            this.set("disabled", !canExecute);
        },
        _setIsAvailableAttr: function (/*Boolean*/ isAvailable) {
            // summary:
            //      Sets the widget as visible or invisible based on the value of isAvailable
            // tags:
            //      private
            this._set("isAvailable", isAvailable);
            if (this.domNode) {
                domStyle.set(this.domNode, "display", isAvailable ? "" : "none");
                showSeparatorEl(this.domNode, this._commandCategory, isAvailable, this.get("showSeparator"));
            }
        },
        _setShowSeparatorAttr: function (/*Boolean*/ showSeparator) {
            // summary:
            //      Sets the widget separator as visible or invisible based on the value of showSeparator
            // tags:
            //      private
            this._set("showSeparator", showSeparator);
            showSeparatorEl(this.domNode, this._commandCategory, this.get("isAvailable"), showSeparator);
        },
        _setIsExecutingAttr: function (/*Boolean*/ isExecuting) {
            // summary:
            //      Adds or removes a css class while the command is executing
            // tags:
            //      private
            this.isExecutingClass && domClass.toggle(this.domNode, this.isExecutingClass, isExecuting);
        },
        onClick: function () {
            // summary:
            //      Event raised when the button is clicked
            //
            // tags:
            //      public
            this.inherited(arguments);
            this.model && this.model.execute.apply(this.model, arguments);
        }
    });
});
