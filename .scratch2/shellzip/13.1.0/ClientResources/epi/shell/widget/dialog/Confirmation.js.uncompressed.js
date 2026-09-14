define("epi/shell/widget/dialog/Confirmation", [
    // Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    // EPi
    "epi/shell/widget/dialog/Dialog",
    "epi/i18n!epi/shell/ui/nls/episerver.shared"
], function (
// Dojo
declare, lang, 
// EPi
Dialog, sharedResources) {
    return declare([Dialog], {
        // summary:
        //		A modal confirmation dialog widget.
        //
        // tags:
        //      public
        // confirmActionText: [public] String
        //		Label to be displayed for the confirm (positive) action.
        confirmActionText: sharedResources.action.yes,
        // cancelActionText: [public] String
        //		Label to be displayed for the cancel (negative) action.
        cancelActionText: sharedResources.action.no,
        // closeIconVisible: [public] Boolean
        //		Flag which indicates whether the close icon should be visible.
        //		Users are requried to answer the confirmation so don't allow
        //		closing via the close icon.
        closeIconVisible: false,
        // dialogClass: [protected] String
        //		Class to apply to the root DOMNode of the dialog.
        dialogClass: "epi-dialog-confirm",
        getActions: function () {
            // summary:
            //      Overridden from Dialog base to assemble the action collection
            // returns:
            //		A collection of action definitions that can be added to the action pane.
            // tags:
            //      protected
            var confirmButton = {
                name: this._okButtonName,
                label: this.confirmActionText,
                title: null,
                action: lang.hitch(this, this._onConfirm)
            }, cancelButton = {
                name: this._cancelButtonName,
                label: this.cancelActionText,
                title: null,
                action: lang.hitch(this, this._onCancel)
            };
            if (this.setFocusOnConfirmButton) {
                confirmButton.settings = { "class": "Salt", firstFocusable: true };
                cancelButton.settings = { "class": "epi-plain-button" };
                return [cancelButton, confirmButton];
            }
            else {
                confirmButton.settings = { "class": "epi-plain-button" };
                cancelButton.settings = { "class": "Salt", firstFocusable: true };
                return [confirmButton, cancelButton];
            }
        },
        _onConfirm: function () {
            // summary:
            //		Called when user has triggered the dialog's confirm action.
            // type:
            //		callback
            this.hide();
            this.onAction(true);
        },
        _onCancel: function () {
            // summary:
            //		Called when user has triggered the dialog's cancel action.
            // type:
            //		callback
            this.hide();
            this.onAction(false);
        },
        onAction: function ( /*Boolean*/ /*===== confirm =====*/) {
            // summary:
            //		Called when the user has triggered an action on the dialog.
            // type:
            //		public callback
        }
    });
});
