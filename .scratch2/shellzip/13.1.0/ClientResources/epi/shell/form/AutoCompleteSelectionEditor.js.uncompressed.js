define("epi/shell/form/AutoCompleteSelectionEditor", [
    "dojo/_base/declare",
    "epi/shell/store/JsonRest",
    "dijit/form/FilteringSelect"
], function (declare, JsonRest, FilteringSelect) {
    return declare([FilteringSelect], {
        // tags:
        //      internal
        // required: [public] Boolean
        //      True if user is required to enter a value into this field. False by default.
        required: false,
        postMixInProperties: function () {
            var store = new JsonRest({
                target: this.storeurl
            });
            this.set("store", store);
            this.inherited(arguments);
            this.actualValue = this.value;
        },
        destroy: function () {
            // Remove the validtion message in case it has been displayed.
            this.displayMessage();
            this.inherited(arguments);
        },
        _setValueAttr: function (value) {
            // dijit text field does not allow to have null as value and replace it with empty string which triggers save action
            if (value === null && this.value === "") {
                return;
            }
            if (this._onChangeActive) {
                this._pendingOnChange = this.actualValue !== value;
                this.actualValue = value;
                this._pendingOnChange && this.onFocus();
            }
            this.inherited(arguments);
        }
    });
});
