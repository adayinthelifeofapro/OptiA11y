define("epi/shell/dist/contextEventManager", ["require", "exports"], (function (u, o) {
    "use strict";
    function a(e, n, r) { if (!n)
        return function () { }; var t = function (d) { var i = d === null || d === void 0 ? void 0 : d.detail; i && i.type === n && r(i.payload); }; return window.addEventListener(e, t), function () { window.removeEventListener(e, t); }; }
    function c(e, n, r) { if (!n)
        return; var t = new CustomEvent(e, { detail: { type: n, payload: r } }); window.dispatchEvent(t); }
    var s = { change: function (e, n) { return c("epi:context:change", e, n); }, onChange: function (e, n) { return a("epi:context:change", e, n); }, refresh: function (e) { return c("epi:context:refresh", e); }, onRefresh: function (e, n) { return a("epi:context:refresh", e, n); } };
    o.ContextEventManager = s, Object.defineProperty(o, Symbol.toStringTag, { value: "Module" });
}));
//# sourceMappingURL=contextEventManager.js.map
