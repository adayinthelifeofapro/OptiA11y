define("epi/shell/widget/dialog/ContextAwareDialog", [
    // dojo
    "dojo/_base/declare",
    // epi.shell
    "epi/shell/_ContextMixin",
    "epi/shell/widget/dialog/Dialog"
], function (
// dojo
declare, 
// epi.shell
_ContextMixin, Dialog) {
    return declare([Dialog, _ContextMixin], {
        contextChanged: function () {
            this.hide();
            this.destroy();
        }
    });
});
