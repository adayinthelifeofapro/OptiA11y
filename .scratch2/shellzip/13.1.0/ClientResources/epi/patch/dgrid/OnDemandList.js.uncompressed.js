define("epi/patch/dgrid/OnDemandList", [
    "dojo/_base/lang",
    "dojo/when",
    "dgrid/OnDemandList",
    "put-selector/put"
], function (lang, when, OnDemandList, put) {
    // module:
    //		epi/patch/dgrid/OnDemandList
    // summary:
    //		Changed renderHeader method to be able to add title to header columns.
    lang.mixin(OnDemandList.prototype, {
        renderQuery: function (query, options) {
            // summary:
            //		Creates a preload node for rendering a query into, and executes the query
            //		for the first page of data. Subsequent data will be downloaded as it comes
            //		into view.
            var self = this, container = (options && options.container) || this.contentNode, preload = {
                query: query,
                count: 0,
                options: options
            }, preloadNode, priorPreload = this.preload, results;
            // Initial query; set up top and bottom preload nodes
            var topPreload = {
                node: put(container, "div.dgrid-preload", {
                    rowIndex: 0
                }),
                count: 0,
                query: query,
                next: preload,
                options: options
            };
            topPreload.node.style.height = "0";
            preload.node = preloadNode = put(container, "div.dgrid-preload");
            preload.previous = topPreload;
            // this preload node is used to represent the area of the grid that hasn't been
            // downloaded yet
            preloadNode.rowIndex = this.minRowsPerPage;
            if (priorPreload) {
                // the preload nodes (if there are multiple) are represented as a linked list, need to insert it
                if ((preload.next = priorPreload.next) &&
                    // is this preload node below the prior preload node?
                    preloadNode.offsetTop >= priorPreload.node.offsetTop) {
                    // the prior preload is above/before in the linked list
                    preload.previous = priorPreload;
                }
                else {
                    // the prior preload is below/after in the linked list
                    preload.next = priorPreload;
                    preload.previous = priorPreload.previous;
                }
                // adjust the previous and next links so the linked list is proper
                preload.previous.next = preload;
                preload.next.previous = preload;
            }
            else {
                this.preload = preload;
            }
            var loadingNode = put(preloadNode, "-div.dgrid-loading"), innerNode = put(loadingNode, "div.dgrid-below");
            innerNode.innerHTML = this.loadingMessage;
            function errback(err) {
                // Used as errback for when calls;
                // remove the loadingNode and re-throw if an error was passed
                put(loadingNode, "!");
                if (err) {
                    if (self._refreshDeferred) {
                        self._refreshDeferred.reject(err);
                        delete self._refreshDeferred;
                    }
                    throw err;
                }
            }
            // Establish query options, mixing in our own.
            // (The getter returns a delegated object, so simply using mixin is safe.)
            options = lang.mixin(this.get("queryOptions"), options, { start: 0, count: this.minRowsPerPage }, "level" in query ? { queryLevel: query.level } : null);
            // Protect the query within a _trackError call, but return the QueryResults
            this._trackError(function () { return results = query(options); });
            if (typeof results === "undefined") {
                // Synchronous error occurred (but was caught by _trackError)
                errback();
                return;
            }
            // Render the result set
            when(self.renderArray(results, preloadNode, options)).then(function (trs) {
                var total = typeof results.total === "undefined" ?
                    results.length : results.total;
                return when(total).then(function (total) {
                    var trCount = trs.length, parentNode = preloadNode.parentNode, noDataNode = self.noDataNode;
                    put(loadingNode, "!");
                    if (!("queryLevel" in options)) {
                        self._total = total;
                    }
                    // now we need to adjust the height and total count based on the first result set
                    if (total === 0) {
                        if (noDataNode) {
                            put(noDataNode, "!");
                            delete self.noDataNode;
                        }
                        self.noDataNode = noDataNode = put("div.dgrid-no-data");
                        if (parentNode) {
                            parentNode.insertBefore(noDataNode, self._getFirstRowSibling(parentNode));
                        }
                        noDataNode.innerHTML = self.noDataMessage;
                    }
                    var height = 0;
                    for (var i = 0; i < trCount; i++) {
                        height += self._calcRowHeight(trs[i]);
                    }
                    // only update rowHeight if we actually got results and are visible
                    if (trCount && height) {
                        self.rowHeight = height / trCount;
                    }
                    total -= trCount;
                    preload.count = total;
                    preloadNode.rowIndex = trCount;
                    if (total) {
                        preloadNode.style.height = Math.min(total * self.rowHeight, self.maxEmptySpace) + "px";
                    }
                    else {
                        // if total is 0, IE quirks mode can't handle 0px height for some reason, I don't know why, but we are setting display: none for now
                        preloadNode.style.display = "none";
                        // This is a hack to get Observable to recognize that this is the
                        // last page, like is done in the processScroll function
                        options.count++;
                    }
                    if (self._previousScrollPosition) {
                        // Restore position after a refresh operation w/ keepScrollPosition
                        self.scrollTo(self._previousScrollPosition);
                        delete self._previousScrollPosition;
                    }
                    // Redo scroll processing in case the query didn't fill the screen,
                    // or in case scroll position was restored
                    self._processScroll();
                    // If _refreshDeferred is still defined after calling _processScroll,
                    // resolve it now (_processScroll will remove it and resolve it itself
                    // otherwise)
                    if (self._refreshDeferred) {
                        self._refreshDeferred.resolve(results);
                        delete self._refreshDeferred;
                    }
                    return trs;
                }).otherwise(function (error) {
                    console.error(error);
                    errback(arguments);
                });
            }).otherwise(function (error) {
                console.error(error);
                errback(arguments);
            });
            return results;
        }
    });
});
