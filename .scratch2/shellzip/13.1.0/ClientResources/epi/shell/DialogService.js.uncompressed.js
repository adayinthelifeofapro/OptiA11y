define("epi/shell/DialogService", [
    "dojo/_base/declare",
    "dojo/aspect",
    "dojo/Deferred",
    "dojo/on",
    "dijit/Dialog",
    "dgrid/List",
    "dgrid/extensions/DijitRegistry",
    "put-selector/put",
    "epi/epi",
    "epi/shell/widget/dialog/Alert",
    "epi/shell/widget/dialog/Confirmation",
    "epi/shell/widget/dialog/Dialog"
], function (declare, aspect, Deferred, on, DijitDialog, List, DijitRegistry, put, epi, Alert, Confirmation, Dialog) {
    var ErrorList = declare([List, DijitRegistry], {
        renderRow: function (value, options) {
            return put("div", "", {
                innerHTML: value
            });
        }
    });
    var module = {
        // summary:
        //		Show different dialogs.
        //
        // description:
        //		Use the "show" method to pick the type of dialog by its name (useful for data objects where
        //      the message type is a part of it) or use the stand-alone methods to easily show a dialog with sensible
        //      defaults.
        //
        // tags:
        //      public
        _isInitialzied: false,
        _initialize: function () {
            // summary:
            //      Update dialog init value into stack whenever show.
            if (this._isInitialzied) {
                return;
            }
            this._isInitialzied = true;
            aspect.after(DijitDialog._DialogLevelManager, "show", function (dialog, underlayAttrs) {
                var stacks = dialog && DijitDialog._dialogStack.filter(function (s) {
                    return s && s.dialog && s.dialog.id === dialog.id;
                });
                if (!stacks || !Array.isArray(stacks) || stacks.length !== 1) {
                    return;
                }
                var forms = dialog._getDescendantFormWidgets();
                if (!forms || !Array.isArray(forms) || forms.length <= 0) {
                    return;
                }
                stacks[0].initValues = {};
                forms.forEach(function (f) {
                    stacks[0].initValues[f.id] = f.value || f.get("value");
                    dialog.own(aspect.after(f, "_setValueAttr", function () {
                        stacks[0].initValues[f.id] = f.value || f.get("value");
                    }));
                });
            }, true);
        },
        hasPendingChanges: function (dialogStack, compareOptions) {
            // summary:
            //      Check dialog value is changed or not.
            // tags:
            //      public
            var forms = dialogStack && dialogStack.dialog && dialogStack.dialog._getDescendantFormWidgets();
            if (!forms || !Array.isArray(forms) || forms.length <= 0) {
                return false;
            }
            return !forms.every(function (f) {
                var value = f.get("value"), initValue = dialogStack.initValues[f.id];
                if (typeof value === "string" || typeof initValue === "string") {
                    return epi.areEqual(value, initValue, compareOptions);
                }
                // compare deep only if initial value is an object
                if (typeof value !== "object" || typeof initValue !== "object") {
                    return false;
                }
                return Object.keys(value).every(function (key) {
                    return epi.areEqual(value[key], initValue[key], compareOptions);
                });
            });
        },
        show: function (type, settings) {
            // summary:
            //      Convenience method to show a dialog based on the "type" instead of calling the specific method.
            // description:
            //      type: The type of dialog. I.e. alert, confirm.
            //      settings: Include "title" and "description" to set the text. Optionally override the default settings.
            //      Returns a promise.
            // tags:
            //      public
            if (!this[type]) {
                throw new Error("Dialog type " + type + " is not a valid DialogService option.");
            }
            return this[type].call(this, settings);
        },
        alert: function (settings) {
            // summary:
            //      Displays an Alert dialog.
            // settings: [Object|string]
            //      Include "title" and "description" to set the text. Optionally override the default settings.
            //      when settings is a string then used as a description
            // returns:
            //      Returns a promise. Always resolves.
            // tags:
            //      public
            if (typeof settings === "string") {
                settings = {
                    description: settings
                };
            }
            return this.alertWithErrors(settings);
        },
        alertWithErrors: function (settings, errors) {
            // summary:
            //      Displays an Alert dialog with a list errors
            // settings: Object
            //      Include "heading" and "description" to set the text. Optionally override the default settings.
            // errors:  [string]
            //      A list of errors to display, it will replace the description passed in the settings object
            // returns:
            //      Returns a promise. Always resolves.
            // tags:
            //      public
            var deferred = new Deferred();
            if (errors instanceof Array) {
                var list = new ErrorList({ className: "epi-grid-max-height--300" });
                list.renderArray(errors);
                list.startup();
                settings.content = list;
                settings.description = null;
            }
            this._callDialog(Alert, settings, deferred.resolve);
            return deferred.promise;
        },
        confirmation: function (settings) {
            // summary:
            //      Displays an Confirmation dialog.
            // description:
            //      settings: Include "title" and "description" to set the text. Optionally override the default settings.
            //      Returns a promise. Resolves on confirm, rejects on cancel.
            // tags:
            //      public
            var deferred = new Deferred();
            this._callDialog(Confirmation, settings, function (confirm) {
                if (confirm) {
                    deferred.resolve();
                }
                else {
                    deferred.reject();
                }
            });
            return deferred.promise;
        },
        dialog: function (settings) {
            // summary:
            //      Displays a dialog.
            // description:
            //      settings: Include "title" and "description" to set the text. Optionally override the default settings.
            //      Returns a promise. Resolves on dialog submit, rejects on cancel.
            // tags:
            //      internal
            var deferred = new Deferred();
            var dialog = new Dialog(settings);
            on.once(dialog, "execute", function (value) {
                deferred.resolve(value);
            });
            on.once(dialog, "cancel", function () {
                deferred.reject();
            });
            dialog.show();
            return deferred.promise;
        },
        _callDialog: function (dialogClass, settings, callback) {
            var dialog = new dialogClass(settings);
            dialog.on("action", callback);
            dialog.show();
        },
        _isInDialog: function (widget, dialog) {
            // summary:
            //      Check if widget is belong to a dialog or not.
            // tags:
            //      internal
            return widget && widget.domNode && dialog && dialog.domNode && dialog.domNode.contains(widget.domNode);
        },
        getEnclosingDialog: function (widget) {
            // summary:
            //      Get closet dialog that the widget is belong to.
            // tags:
            //      internal
            return DijitDialog._dialogStack && DijitDialog._dialogStack.filter(function (x) {
                return this._isInDialog(widget, x.dialog);
            }.bind(this))[0];
        },
        findInstancesOfType: function (dialog, checkInheritanceChain) {
            // summary:
            //      Get the list of dialogs instances that is or inheritance from the dialog.
            // tags:
            //      internal
            return DijitDialog._dialogStack && DijitDialog._dialogStack.filter(function (x) {
                return x && x.dialog && x.dialog.constructor === dialog.constructor || (checkInheritanceChain && x.dialog.constructor._meta.bases.some(function (y) {
                    return y === dialog.constructor;
                }));
            });
        },
        hideStack: function (dialogStacks) {
            // summary:
            //      Hide all dialog inside the stacks.
            //      Warning: If dialogStacks is not set then all current stacks will be hidden.
            // tags:
            //      internal
            dialogStacks = dialogStacks || DijitDialog._dialogStack;
            Array.isArray(dialogStacks) && dialogStacks.forEach(function (stack) {
                stack && stack.dialog && stack.dialog.hide();
            });
        }
    };
    module._initialize();
    return module;
});
