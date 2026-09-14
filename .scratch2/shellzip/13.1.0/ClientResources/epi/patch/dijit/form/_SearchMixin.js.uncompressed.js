define("epi/patch/dijit/form/_SearchMixin", [
    "dojo/_base/lang",
    "dojo/string",
    "dojo/when",
    "dojo/data/util/filter",
    "dijit/form/_SearchMixin"
], function (lang, string, when, filter, _SearchMixin) {
    lang.mixin(_SearchMixin.prototype, {
        _startSearch: function (/*String*/ text) {
            // summary:
            //		Starts a search for elements matching text (text=="" means to return all items),
            //		and calls onSearch(...) when the search completes, to display the results.
            this._abortQuery();
            var _this = this, 
            // Setup parameters to be passed to store.query().
            // Create a new query to prevent accidentally querying for a hidden
            // value from FilteringSelect's keyField
            query = lang.clone(this.query), // #5970
            options = {
                start: 0,
                count: this.pageSize,
                queryOptions: {
                    ignoreCase: this.ignoreCase,
                    deep: true
                }
            }, qs = string.substitute(this.queryExpr, [text.replace(/([\\\*\?])/g, "\\$1")]), 
            /* THE FIX GOES HERE */
            /* --------------------------------------------------------------------------------------------- */
            qToString = string.substitute(this.queryExpr, [text]), 
            /* --------------------------------------------------------------------------------------------- */
            /* END FIX */
            q, startQuery = function () {
                var resPromise = _this._fetchHandle = _this.store.query(query, options);
                if (_this.disabled || _this.readOnly || (q !== _this._lastQuery)) {
                    return;
                } // avoid getting unwanted notify
                when(resPromise, function (res) {
                    _this._fetchHandle = null;
                    if (!_this.disabled && !_this.readOnly && (q === _this._lastQuery)) { // avoid getting unwanted notify
                        when(resPromise.total, function (total) {
                            res.total = total;
                            var pageSize = _this.pageSize;
                            if (isNaN(pageSize) || pageSize > res.total) {
                                pageSize = res.total;
                            }
                            // Setup method to fetching the next page of results
                            res.nextPage = function (direction) {
                                //	tell callback the direction of the paging so the screen
                                //	reader knows which menu option to shout
                                options.direction = direction = direction !== false;
                                options.count = pageSize;
                                if (direction) {
                                    options.start += res.length;
                                    if (options.start >= res.total) {
                                        options.count = 0;
                                    }
                                }
                                else {
                                    options.start -= pageSize;
                                    if (options.start < 0) {
                                        options.count = Math.max(pageSize + options.start, 0);
                                        options.start = 0;
                                    }
                                }
                                if (options.count <= 0) {
                                    res.length = 0;
                                    _this.onSearch(res, query, options);
                                }
                                else {
                                    startQuery();
                                }
                            };
                            _this.onSearch(res, query, options);
                        });
                    }
                }, function (err) {
                    _this._fetchHandle = null;
                    if (!_this._cancelingQuery) { // don't treat canceled query as an error
                        console.error(_this.declaredClass + ' ' + err.toString());
                    }
                });
            };
            lang.mixin(options, this.fetchProperties);
            // Generate query
            if (this.store._oldAPI) {
                // remove this branch for 2.0
                q = qs;
            }
            else {
                // Query on searchAttr is a regex for benefit of dojo/store/Memory,
                // but with a toString() method to help dojo/store/JsonRest.
                // Search string like "Co*" converted to regex like /^Co.*$/i.
                q = filter.patternToRegExp(qs, this.ignoreCase);
                q.toString = function () {
                    /* THE FIX GOES HERE */
                    /* --------------------------------------------------------------------------------------------- */
                    return qToString;
                    /* --------------------------------------------------------------------------------------------- */
                    /* END FIX */
                };
            }
            // set _lastQuery, *then* start the timeout
            // otherwise, if the user types and the last query returns before the timeout,
            // _lastQuery won't be set and their input gets rewritten
            this._lastQuery = query[this.searchAttr] = q;
            this._queryDeferHandle = this.defer(startQuery, this.searchDelay);
        }
    });
});
