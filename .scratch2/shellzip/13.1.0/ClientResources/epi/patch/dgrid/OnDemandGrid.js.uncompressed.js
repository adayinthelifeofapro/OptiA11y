define("epi/patch/dgrid/OnDemandGrid", [
    "dojo/aspect",
    "dgrid/OnDemandGrid"
], function (aspect, OnDemandGrid) {
    // module:
    //		epi/patch/dgrid/OnDemandGrid
    // summary:
    //		Changed renderHeader method to be able to add title to header columns.
    aspect.after(OnDemandGrid.prototype, "renderHeader", function () {
        for (var key in this.columns) {
            if (Object.hasOwnProperty.call(this.columns, key)) {
                var column = this.columns[key];
                column.headerNode.title = column.label || column.field;
            }
        }
    });
});
