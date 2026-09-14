define("epi/patch/dojox/form/uploader/plugins/HTML5", [
    "dojo/_base/lang",
    "dojox/form/Uploader",
    "dojox/form/uploader/plugins/HTML5",
    "epi/i18n!epi/cms/nls/episerver.cms.widget.uploadmultiplefiles"
], function (lang, Uploader, HTML5, res) {
    // module:
    //		dojox/form/uploader/plugins/HTML5
    // summary:
    //		Fix issue with unhandled error when parsing non-JSON data as JSON
    lang.mixin(HTML5.prototype, {
        createXhr: function () {
            var xhr = new XMLHttpRequest();
            var timer;
            xhr.upload.addEventListener("progress", lang.hitch(this, "_xhrProgress"), false);
            xhr.addEventListener("load", lang.hitch(this, "_xhrProgress"), false);
            xhr.addEventListener("error", lang.hitch(this, function (evt) {
                this.onError(evt);
                clearInterval(timer);
            }), false);
            xhr.addEventListener("abort", lang.hitch(this, function (evt) {
                this.onAbort(evt);
                clearInterval(timer);
            }), false);
            xhr.onreadystatechange = lang.hitch(this, function () {
                if (xhr.readyState === 4) {
                    //				console.info("COMPLETE")
                    clearInterval(timer);
                    /* THE FIX GOES HERE */
                    /* --------------------------------------------------------------------------------------------- */
                    // Handle too large file exception in onreadystatechange
                    // because onerror event is fired on network errors and not on status codes
                    // (we need to handle both scenarios, for local environment onerror, otherwise onreadystatechange)
                    // Add handling for status codes 4xx and 5xx in general, not only 413.
                    if (xhr.status >= 400 && xhr.status < 600) {
                        this.onError({
                            message: res.uploadform.failed,
                            data: xhr.responseText,
                            statusCode: xhr.status,
                            headers: xhr.getAllResponseHeaders()
                        });
                        return;
                    }
                    // The readyState value of 4 means the operation completed
                    //(successfully or failed).The status property is initialized to 0
                    //and will remain at 0 if an error occurs, usually when the connection is closed or timeout.
                    if (xhr.status === 0) {
                        this.onError({
                            message: res.uploadform.failed,
                            data: xhr.responseText,
                            statusCode: xhr.status,
                            headers: xhr.getAllResponseHeaders()
                        });
                        return;
                    }
                    // Handle exception when parsing data and trigger onError if there was any error
                    var jsonData;
                    if (xhr.responseText) {
                        try {
                            jsonData = JSON.parse(xhr.responseText.replace(/^\{\}&&/, ''));
                        }
                        catch (e) {
                            this.onError({
                                message: e.message,
                                data: xhr.responseText,
                                statusCode: xhr.status,
                                headers: xhr.getAllResponseHeaders()
                            });
                            return;
                        }
                    }
                    this.onComplete(jsonData);
                    /* --------------------------------------------------------------------------------------------- */
                    /* END FIX */
                }
            });
            xhr.open("POST", this.getUrl());
            timer = setInterval(lang.hitch(this, function () {
                try {
                    //  accessing this error throws an error. Awesomeness.
                    if (typeof (xhr.statusText)) { } //eslint-disable-line no-empty
                }
                catch (e) {
                    //this.onError("Error uploading file."); // not always an error.
                    clearInterval(timer);
                }
            }), 250);
            return xhr;
        }
    });
    HTML5.prototype.createXhr.nom = "createXhr";
});
