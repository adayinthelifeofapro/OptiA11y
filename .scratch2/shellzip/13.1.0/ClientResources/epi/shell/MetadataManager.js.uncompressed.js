define("epi/shell/MetadataManager", [
    "dojo/_base/declare",
    "dojo/_base/json",
    "dojo/Deferred",
    "dojo/when",
    "epi/dependency",
    "./PropertyMetadata"
], function (declare, json, Deferred, when, dependency, PropertyMetadata) {
    return declare(null, {
        // summary:
        //     Manage metadata that build a form
        //
        // tags:
        //      public
        store: null,
        _setTypeMetadataStore: function () {
            // summary:
            //     Sets up the default store used to communicate with the server.
            var registry = dependency.resolve("epi.storeregistry");
            this.store = registry.get("epi.shell.metadata");
        },
        getMetadataForType: function (/*String*/ type, /*Object?*/ params, /*Boolean?*/ force) {
            // summary:
            //     Returns a <see cref="dojo/Deferred" /> promise, which in turn returns a metadata object
            //     that can be used to create a UI to edit the object type.
            //
            // type: String
            //     The object type to return metadata for. For example:
            //     EPiServer.Shell.ViewComposition.Containers.BorderContainer
            //
            // params: Object
            //     Additional parameters to create metadata. For example:
            //     PageMetadataProvider needs a PageData instance when creating its metadata, then we provide pageGuid and pageLang here.
            //
            // force: Boolean
            //     Query from server and bypass the cache layer
            function onItem(metaData) {
                return metaData ? new PropertyMetadata(metaData) : metaData;
            }
            if (this.store === null) {
                this._setTypeMetadataStore();
            }
            var self = this;
            if (params) {
                var def = new Deferred();
                when(this.store.query({ id: type, modelAccessor: json.toJson(params) }), function (metadata) {
                    if (metadata && metadata.length) {
                        if (params["contentItemLocator"]) {
                            metadata[0].contentItemLocator = params["contentItemLocator"];
                        }
                        else if (params["contentLink"]) {
                            metadata[0].contentItemLocator = [params["contentLink"]];
                        }
                        self._propagateParentLocator(metadata[0].properties, metadata[0].contentItemLocator);
                        def.resolve(onItem(metadata[0]));
                    }
                    else {
                        def.reject();
                    }
                }, function (error) {
                    def.reject(error);
                });
                return def.promise;
            }
            else if (force) {
                return when(this.store.query({ id: type }), onItem);
            }
            else {
                return when(this.store.get(type), onItem);
            }
        },
        _clear: function () {
            this.store = null;
        },
        _propagateParentLocator: function (properties, parentLocator) {
            var _this = this;
            if (!properties || properties.length === 0 || !parentLocator) {
                return;
            }
            properties.forEach(function (x) {
                if (x.settings) {
                    x.settings.parentLocator = parentLocator;
                    _this._propagateParentLocator(x.properties, x.settings.parentLocator.concat(x.name));
                }
            });
        }
    });
});
