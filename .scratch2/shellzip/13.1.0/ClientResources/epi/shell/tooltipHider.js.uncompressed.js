define("epi/shell/tooltipHider", [
    "dojo/on",
    "dijit/Tooltip",
    "dijit/registry",
    "epi/debounce"
], function (on, Tooltip, registry, debounce) {
    // summary:
    //      Watcher of any dijit/Tooltip instances created by the provided widget
    //      Once applied to any widget it would track the tooltips created and hide them when the
    //      widget's dom node is scrolled.
    //      ref. https://bugs.dojotoolkit.org/ticket/5777
    // tags:
    //      internal
    var activeTooltips = [];
    var originalOpen = Tooltip._MasterTooltip.prototype.show;
    Tooltip._MasterTooltip.prototype.show = function () {
        originalOpen.apply(this, arguments);
        activeTooltips.push(this.get("id"));
    };
    Tooltip._MasterTooltip.prototype.show.nom = "show";
    var originalClose = Tooltip._MasterTooltip.prototype.hide;
    Tooltip._MasterTooltip.prototype.hide = function () {
        originalClose.apply(this, arguments);
        var tooltipIdIndex = activeTooltips.indexOf(this.get("id"));
        if (tooltipIdIndex !== -1) {
            activeTooltips.splice(tooltipIdIndex, 1);
        }
    };
    Tooltip._MasterTooltip.prototype.hide.nom = "hide";
    function hideAll() {
        // iterating backwards because we are using the 'hide' method which removes tooltip widgets from the activeTooltips array
        for (var i = activeTooltips.length - 1; i >= 0; i--) {
            var tooltipId = activeTooltips[i];
            var widget = registry.byId(tooltipId);
            if (widget) {
                widget.hide(widget.aroundNode);
            }
        }
    }
    return function (widget) {
        if (!widget || !widget.domNode) {
            return;
        }
        widget.own(on(widget.domNode, "scroll", debounce(hideAll, null, 100)));
    };
});
