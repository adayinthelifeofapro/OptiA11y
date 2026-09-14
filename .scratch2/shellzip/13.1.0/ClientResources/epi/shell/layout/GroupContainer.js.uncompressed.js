define("epi/shell/layout/GroupContainer", [
    "require",
    // dojo
    "dojo/_base/declare",
    "dojo/dom-construct",
    // epi
    "./SimpleContainer"
], function (moduleRequire, 
// dojo
declare, domConstruct, 
// epi
SimpleContainer) {
    return declare([SimpleContainer], {
        // summary:
        //      Widget that contains a group form items widgets, aimed to apply the html formatting specific to a group
        //
        // tags:
        //      internal
        labelNode: null,
        templateString: "<fieldset>\
                <ul class=\"epi-form-container__section\" data-dojo-attach-point=\"containerNode\"></ul>\
                <div data-dojo-attach-point=\"extraInforContainerNode\"></div>\
            </fieldset>",
        _ensureLabelNode: function () {
            if (!this.labelNode) {
                this.labelNode = document.createElement("legend");
                this.domNode.prepend(this.labelNode);
            }
        },
        _setTitleAttr: function (value) {
            if (!value && !this.labelNode) {
                return;
            }
            this._ensureLabelNode();
            if (!this.labelTextNode) {
                this.labelTextNode = document.createElement("span");
                this.labelNode.prepend(this.labelTextNode);
            }
            this.labelTextNode.innerHTML = value;
        },
        _setNameAttr: function (value) {
            if (!value) {
                return;
            }
            this.containerNode.dataset.groupName = value;
        },
        _setTooltipAttr: function (value) {
            if (!value) {
                if (this.tooltipNode && this.tooltipNode.parent) {
                    this.tooltipNode.parent.removeChild(this.tooltipNode);
                    this.tooltipNode = undefined;
                }
                return;
            }
            this._ensureLabelNode();
            moduleRequire([
                "epi-cms-react/components/property-help-text-widget",
                "xstyle/css!epi-cms-react/components/property-help-text-widget.css"
            ], function (PropertyHelpText) {
                if (!this.tooltipNode) {
                    this.tooltipNode = document.createElement("div");
                    this.tooltipNode.classList.add("epi-form-container__section__tooltip");
                    this.labelNode.appendChild(this.tooltipNode);
                }
                if (this._tooltipCreated || !value) {
                    return;
                }
                var helpTextWidget = new PropertyHelpText({
                    helpText: value
                });
                helpTextWidget.placeAt(this.tooltipNode);
                this.own(helpTextWidget);
                this._tooltipCreated = true;
            }.bind(this));
        }
    });
});
