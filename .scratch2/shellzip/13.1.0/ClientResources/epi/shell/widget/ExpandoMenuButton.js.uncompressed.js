require({cache:{
'url:epi/shell/widget/templates/ExpandoMenuButton.html':"﻿<span class=\"dijit dijitReset dijitInline epi-mediumButton epi-expandoMenuButton\"><!--\n --><button data-dojo-attach-point=\"toggleButton\" data-dojo-type=\"dijit/form/ToggleButton\" class=\"epi-chromelessButton\"\n        data-dojo-attach-event=\"onChange: _toggleButtonChange, onClick:_toggleButtonClick\" showlabel=\"false\">${label}</button><!--\n    --><ul data-dojo-attach-point=\"expandoNode\" class=\"dijitHidden\"><li data-dojo-attach-point=\"innerTextContainerNode\" class=\"epi-expandoMenuButton--no-children\"><span class=\"epi-expandoMenuButton--no-children-text\" data-dojo-attach-point=\"innerTextNode\"></span></li></ul><!--\n--></span>\n"}});
define("epi/shell/widget/ExpandoMenuButton", [
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
    "dojo/dom-geometry",
    "dojo/dom-style",
    "dojo/dom-construct",
    "dojo/has",
    "dojo/text!./templates/ExpandoMenuButton.html",
    "dijit/registry",
    "dijit/_WidgetBase",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/_TemplatedMixin",
    "dgrid/util/has-css3",
    // Widgets used in the template
    "dijit/form/ToggleButton"
], function (declare, lang, array, on, domClass, domGeometry, domStyle, domConstruct, has, template, registry, _WidgetBase, _WidgetsInTemplateMixin, _TemplatedMixin) {
    return declare([_WidgetBase, _TemplatedMixin, _WidgetsInTemplateMixin], {
        // summary:
        //    A toggle button wrapping child buttons inside a slide-out panel only visible when enabled/checked
        //
        // tags:
        //    internal
        // templateString: [protected] String
        //      Widget's template string.
        templateString: template,
        // value: [public] String
        //      Represents the label/tooltip of this button.
        label: "",
        // tooltip: [public] String
        //      Represents the tooltip of this button.
        tooltip: "",
        // showLabel: [public] Boolean
        //      Flags that represents the show or hide the label.
        showLabel: false,
        // innerText: [public] String
        //      The text displayed if there are no children added to the button
        innerText: "",
        iconClass: "",
        // hideSingleOption: [public] Boolean
        //      Flag to indicate whether the children should be hidden
        //      when the button has only 1 child, and that child has only 1 option
        hideSingleOption: false,
        postCreate: function () {
            // summary:
            //      Post widget creation.
            // tags:
            //      protected
            this.inherited(arguments);
            if (has("css-transitions")) {
                this.own(on(this.expandoNode, has("transitionend"), lang.hitch(this, this._onTransitionEnd)));
            }
        },
        _setLabelAttr: function (label) {
            this.toggleButton.set("label", label);
            this._set("label", label);
        },
        _setTooltipAttr: function (tooltip) {
            this.toggleButton.titleNode.title = tooltip;
            this._set("tooltip", tooltip);
            if (!this.showLabel && !("title" in this.params)) {
                this.toggleButton.titleNode.title = (this.toggleButton.containerNode.innerText || this.toggleButton.containerNode.textContent || "").trim();
            }
        },
        _setShowLabelAttr: function (value) {
            this.toggleButton.set("showLabel", value);
            this._set("showLabel", value);
        },
        _setInnerTextAttr: function (innerText) {
            this._set("innerText", innerText);
            if (innerText) {
                this.innerTextNode.textContent = innerText;
            }
            domClass.toggle(this.innerTextContainerNode, "dijitHidden", !innerText);
        },
        _setIconClassAttr: function (iconClass) {
            this._set("iconClass", iconClass);
            this._updateToggleButtonIcon();
        },
        _setCheckedAttr: function (isChecked) {
            if (isChecked) {
                this.domNode.classList.add("epi-expandoMenuButton--active");
                var activeMessage = this.settings && this.settings.statusMessages && this.settings.statusMessages.active;
                if (activeMessage) {
                    this.toggleButton.set("label", activeMessage);
                }
            }
            else {
                this.toggleButton.set("label", this.label);
                this.domNode.classList.remove("epi-expandoMenuButton--active");
            }
            this.toggleButton.set("checked", isChecked);
            this._set("checked", isChecked);
            this._updateToggleButtonIcon();
            this._showChildren(isChecked);
        },
        addChild: function (widget) {
            // summary:
            //      Add the widget as a child node.
            // Hide the innerText node if a new child is added
            if (this.innerTextContainerNode) {
                domClass.add(this.innerTextContainerNode, "dijitHidden");
            }
            var li = domConstruct.create("li", {}, this.expandoNode);
            widget.placeAt(li);
        },
        onClick: function () {
            // summary:
            //      Will be raised whenever the view setting button is clicked.
        },
        onChange: function (val) {
            // summary:
            //      Will be raised when the view setting button state changes.
        },
        getChildren: function () {
            return registry.findWidgets(this.expandoNode);
        },
        _toggleButtonClick: function () {
            // tags:
            //      private
            this.onClick();
        },
        _toggleButtonChange: function (val) {
            // summary:
            //      Toggles visibility of child buttons and raises onChange
            // tags:
            //      private
            this.set("checked", val);
            this.onChange(val);
        },
        _updateToggleButtonIcon: function () {
            // summary:
            //      Updates the icon on the toggle button based on our current state.
            // tags:
            //      private
            var iconClasses = [];
            // Add the icon class if there is one.
            if (this.iconClass) {
                iconClasses.push(this.iconClass);
            }
            // Add the active state class if checked is true.
            if (this.checked) {
                iconClasses.push("epi-icon--active");
            }
            this.toggleButton.set("iconClass", iconClasses.join(" "));
        },
        _shouldHideChildren: function () {
            if (!this.hideSingleOption) {
                return false;
            }
            var self = this;
            var childrenCount = function () {
                var children = self.getChildren();
                return children ? children.length : 0;
            };
            var firstChildOptionsCount = function () {
                var firstChild = self.getChildren()[0];
                var options = firstChild.model.get("options");
                return options ? options.length : 0;
            };
            return (childrenCount() === 1) && (firstChildOptionsCount() === 1);
        },
        /*
             *  CSS transition support
             */
        _showChildren: function (show) {
            // summary:
            //      Show/Hide the children using css animation
            var node = this.expandoNode;
            show = show && !this._shouldHideChildren();
            if (show) {
                domClass.toggle(node, "dijitHidden", false);
                domStyle.set(node, "width", this._childrenWidth() + "px");
            }
            else {
                domStyle.set(node, "width", this._childrenWidth() + "px");
                // Force repaint
                node.offsetWidth; // eslint-disable-line no-unused-expressions
                domStyle.set(node, "width", "0px");
            }
        },
        _onTransitionEnd: function () {
            var visible = this.get("checked") && !this._shouldHideChildren();
            // Must be set to display:none after transition to not take part in keyboard navigation
            domClass.toggle(this.expandoNode, "dijitHidden", !visible);
            if (visible) {
                // Let it auto-size
                domStyle.set(this.expandoNode, "width", "");
            }
        },
        _childrenWidth: function () {
            // summary:
            //      Calculates and returns the total width of the children
            var width = 0;
            array.forEach(this.expandoNode.childNodes, function (child) {
                // Children are wrapped in <li> which will be the domNodes parent node.
                width += domGeometry.getMarginBox(child).w;
            });
            return width;
        }
    });
});
