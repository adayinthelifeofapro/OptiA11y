require({cache:{
'url:epi/shell/form/templates/Field.html':"<li class=\"epi-form-container__section__row epi-form-container__section__row--field\">\n    <div class=\"epi-form-container__section__label-container\">\n        <label class=\"epi-form-container__section__label\" data-dojo-attach-point=\"labelNode\">\n            <span class=\"dijitInline dijitReset dijitIcon epi-iconPenDisabled\" data-dojo-attach-point=\"readonlyIcon\"></span>\n            <span class=\"dijitInline dijitReset dijitHidden bound-property\" data-dojo-attach-point=\"boundedIcon\">\n                <span class=\"dijitInline dijitReset dijitIcon epi-iconLink\"></span>\n            </span>\n            <span class=\"epi-required-indicator dijitHidden dijitInline dijitReset\" data-dojo-attach-point=\"requiredIcon\"></span>\n        </label>\n        <div data-dojo-attach-point=\"propertyTooltip\" class=\"epi-form-container__section__tooltip\"></div>\n    </div>\n</li>\n"}});
define("epi/shell/form/Field", [
    // dojo
    "require",
    "dojo/_base/declare",
    "dojo/when",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dijit/_WidgetBase",
    "dijit/_Container",
    "dijit/_Contained",
    "dijit/_TemplatedMixin",
    "dojox/html/entities",
    // epi
    "epi/dependency",
    "epi/shell/layout/_ExtraInforWidgetMixin",
    "./formFieldRegistry",
    "dojo/text!./templates/Field.html",
    "epi/i18n!epi/cms/nls/episerver.shared.validation",
    "epi/i18n!epi/cms/nls/episerver.cms.contentbinding"
], function (
// dojo
moduleRequire, declare, when, domClass, domConstruct, domStyle, _WidgetBase, _Container, _Contained, _TemplatedMixin, htmlEntities, 
// epi
dependency, _ExtraInforWidgetMixin, formFieldRegistry, template, validationRes, bindingRes) {
    var module = declare([_WidgetBase, _Container, _Contained, _TemplatedMixin, _ExtraInforWidgetMixin], {
        // summary:
        //      Sets attributes on field
        // tags:
        //      internal
        labelTarget: "",
        label: "",
        readonlyIconDisplay: null,
        hasFullWidthValue: false,
        isBounded: undefined,
        templateString: template,
        postMixInProperties: function () {
            this.inherited(arguments);
            this.messageService = this.messageService || dependency.resolve("epi.shell.MessageService");
        },
        addChild: function (child, index) {
            if (child.checkbox) {
                this._addCheckboxChild(child, this.labelNode);
            }
            else {
                this.inherited(arguments);
            }
            var propertyName = this.get("name");
            if (propertyName) {
                var customNotifications = [];
                if (this.isBounded === false) {
                    customNotifications.push({
                        text: bindingRes.badgetitlenotloaded,
                        type: "warn"
                    });
                }
                this._buildExtraInfoWidget(propertyName, customNotifications);
            }
        },
        _addCheckboxChild: function (child, labelNode) {
            domClass.toggle(this.domNode, "epi-form-container__section__row--checkbox", true);
            domConstruct.place(child.domNode, labelNode, "before");
            if (this._started && !child._started) {
                child.startup();
            }
        },
        _setReadonlyIconDisplayAttr: function (/* Boolean */ value) {
            this._set("readonlyIconDisplay", value);
            domStyle.set(this.readonlyIcon, "display", value === true ? "" : "none");
        },
        _setBoundIconDisplayAttr: function (/* Boolean */ value) {
            this._set("boundIconDisplay", value);
            this.boundedIcon.classList.toggle("dijitHidden", !value);
        },
        _setLabelAttr: function (value) {
            this._set("label", value);
            this.labelNode.insertBefore(domConstruct.toDom(value), this.readonlyIcon);
            domClass.toggle(this.labelNode, "dijitHidden", !value);
            this.requiredIcon.title = validationRes.required.replace("{0}", value);
        },
        _setLabelTargetAttr: {
            node: "labelNode",
            attribute: "for",
            type: "attribute"
        },
        _setNameAttr: function (value) {
            this._set("name", value);
            if (value) {
                this.domNode.dataset.propertyName = value.toLowerCase();
            }
        },
        _setTooltipAttr: function (value) {
            moduleRequire([
                "epi-cms-react/components/property-help-text-widget",
                "xstyle/css!epi-cms-react/components/property-help-text-widget.css"
            ], function (PropertyHelpText) {
                if (!this.propertyTooltip) {
                    return;
                }
                if (this._tooltipCreated || !value) {
                    return;
                }
                var helpTextWidget = new PropertyHelpText({
                    helpText: value
                });
                helpTextWidget.placeAt(this.propertyTooltip);
                this.own(helpTextWidget);
                this._tooltipCreated = true;
            }.bind(this));
        },
        _setHasFullWidthValueAttr: function (/* Boolean */ value) {
            value && domClass.add(this.containerNode, "epi-form-container__section__row--full-width");
        },
        _setRequiredAttr: function (/* Boolean */ value) {
            this.requiredIcon.classList.toggle("dijitHidden", !value);
        }
    });
    formFieldRegistry.add({
        type: formFieldRegistry.type.field,
        hint: "",
        factory: function (widget, parent) {
            var wrapper = new parent._FieldItem({
                name: widget.name,
                labelTarget: widget.checkbox ? widget.checkbox.id : widget.id,
                label: widget.label,
                tooltip: widget.tooltip,
                readonlyIconDisplay: widget.readOnly && !widget.boundProperty,
                boundIconDisplay: widget.boundProperty,
                isBounded: widget.isBounded,
                hasFullWidthValue: widget.useFullWidth,
                required: widget.required
            });
            if (widget.required) {
                // change label color to error when required field is in error state
                wrapper.own(widget.watch("state", function (name, oldValue, newValue) {
                    wrapper.labelNode.classList.toggle("error-state", newValue === "Error");
                }));
            }
            wrapper.own(widget.watch("readOnly", function (name, oldValue, newValue) {
                wrapper.set("readonlyIconDisplay", newValue);
            }));
            return wrapper;
        }
    });
    return module;
});
