define("epi/shell/dist/messagePublisher", ["require", "exports"], (function (f, o) {
    "use strict";
    function t(e) { try {
        return Array.from(e.frames);
    }
    catch (_a) {
        return [];
    } }
    function c() { var e = [window, window.top]; return t(window.top).forEach(function (r) { e.push(r), t(r).forEach(function (s) { e.push(s); }); }), Array.from(new Set(e)); }
    function i(e, r) { c().forEach(function (s) { try {
        s.postMessage({ id: e, message: r, data: r }, "*");
    }
    catch (u) {
        console.error(u);
    } }); }
    function n(e) { return { fromCmsKey: "".concat(e, ":cms"), fromDeliveryKey: "".concat(e, ":delivery") }; }
    var a = { screenshot: n("/site/screenshot") };
    o.commonMessageTypes = a, o.getMessageKeys = n, o.publish = i, Object.defineProperty(o, Symbol.toStringTag, { value: "Module" });
}));
//# sourceMappingURL=messagePublisher.js.map
