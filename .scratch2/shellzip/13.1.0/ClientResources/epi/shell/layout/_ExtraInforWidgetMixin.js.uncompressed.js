define("epi/shell/layout/_ExtraInforWidgetMixin", [
    // dojo
    "dojo/_base/declare",
    "dojo/when",
    "require",
    // epi
    "epi/dependency",
    "epi/shell/_ContextMixin"
], function (
// dojo
declare, when, moduleRequire, 
// epi
dependency, _ContextMixin) {
    return declare([_ContextMixin], {
        // summary:
        //     Mixin class to reduce code to create extra information region.
        //
        // tags:
        //      internal xproduct
        postMixInProperties: function () {
            this.inherited(arguments);
            this.own(this.messageService = this.messageService || dependency.resolve("epi.shell.MessageService"));
        },
        _buildExtraInfoWidget: function (propertyName, customNotifications) {
            when(this.getCurrentContext()).then(function (context) {
                var messageContext = {
                    contextId: context.id,
                    contextTypeName: context.type
                };
                this.own(this.messageService.observe(messageContext, function (item, removedFrom, insertedInto) {
                    if (!item.externalItemData ||
                        (item.externalItemData &&
                            (item.externalItemData.propertyName.toLowerCase() != propertyName.toLowerCase() ||
                                !item.externalItemData.showToPropertyExtraInfoArea))) {
                        return;
                    }
                    this._createExtraInfoWidget(messageContext, propertyName, customNotifications);
                }.bind(this)));
                this._createExtraInfoWidget(messageContext, propertyName, customNotifications);
            }.bind(this));
        },
        showNotificationList: function (notifications) {
            notifications = notifications || [];
            if (!this._hasAdditionalNotifications && notifications.length === 0
                && (this._systemNotifications || []).length === 0) {
                return;
            }
            notifications = notifications.map(function (notification) {
                if (typeof notification === "string") {
                    return {
                        text: notification,
                        type: "error"
                    };
                }
                return notification;
            });
            this._hasAdditionalNotifications = notifications.length > 0;
            var allNotifications = (this._systemNotifications || []).slice().concat(notifications || []);
            if (this.extraInfoWidget) {
                this.extraInfoWidget.destroyRecursive();
                this.extraInfoWidget = null;
            }
            moduleRequire([
                "epi-cms-react/components/property-extra-info-widget",
                "xstyle/css!epi-cms-react/components/property-extra-info-widget.css"
            ], function (PropertyExtraInfo) {
                var containerNode = this.extraInforContainerNode || this.containerNode;
                if (!containerNode) {
                    return;
                }
                this.extraInfoWidget = new PropertyExtraInfo({
                    messages: allNotifications
                });
                this.extraInfoWidget.placeAt(containerNode, "last");
                this.own(this.extraInfoWidget);
            }.bind(this));
        },
        _createExtraInfoWidget: function (messageContext, propertyName, customNotifications) {
            if (!propertyName) {
                return;
            }
            // query from message service to get validation errors
            when(this.messageService.query(messageContext)).then(function (items) {
                var itemsToShow = items.filter(function (x) {
                    return x.externalItemData && x.externalItemData.propertyName
                        && x.externalItemData.propertyName.toLowerCase() == propertyName.toLowerCase()
                        && x.externalItemData.showToPropertyExtraInfoArea;
                }).map(function (y) {
                    return {
                        text: y.message,
                        type: y.typeName
                    };
                }) || [];
                this._systemNotifications = itemsToShow;
                this.showNotificationList(customNotifications);
            }.bind(this));
        }
    });
});
