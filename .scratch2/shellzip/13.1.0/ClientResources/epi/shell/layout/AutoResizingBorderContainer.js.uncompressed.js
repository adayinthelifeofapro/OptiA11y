define("epi/shell/layout/AutoResizingBorderContainer", [
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dijit/layout/BorderContainer"
], function (declare, lang, BorderContainer) {
    return declare([BorderContainer], {
        // summary:
        //      A BorderContainer that periodically calls resize() after creation
        //      until the layout stabilizes, fixing the issue where the right pane
        //      fails to expand fully on initial load.
        destroy: function () {
            this._clearResizeInterval();
            this.inherited(arguments);
        },
        _clearResizeInterval: function () {
            if (!this._resizeInterval) {
                return;
            }
            clearInterval(this._resizeInterval);
            this._resizeInterval = null;
        },
        postCreate: function () {
            this.inherited(arguments);
            this._resizeInterval = setInterval(lang.hitch(this, function () {
                if (!this.domNode) {
                    return;
                }
                var rect = this.domNode.getBoundingClientRect();
                if (rect.width === 0) {
                    return;
                }
                if (this._lastWidth === rect.width) {
                    // When width is unchanged, skip the resize operation
                    // because it will reset the state.
                    this._clearResizeInterval();
                    return;
                }
                this._lastWidth = rect.width;
                this.resize();
            }), 500);
        }
    });
});
