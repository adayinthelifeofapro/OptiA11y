define("epi/shell/layout/ParentContainer", [
    // dojo
    "dojo/_base/declare",
    // epi
    "epi/shell/layout/_ExtraInforWidgetMixin",
    "epi/shell/layout/GroupContainer"
], function (
// dojo
declare, 
// epi
_ExtraInforWidgetMixin, GroupContainer) {
    return declare([GroupContainer, _ExtraInforWidgetMixin], {
        // summary:
        //     Widget that contains a group form items widgets, aimed to apply the html formatting specific to a group
        //
        // tags:
        //      internal xproduct
        _setNameAttr: function (propertyName) {
            this.inherited(arguments);
            this._buildExtraInfoWidget(propertyName);
        }
    });
});
