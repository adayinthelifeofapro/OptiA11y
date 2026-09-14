// Currently only a placeholder for the core epi namespace
// Needed to get the amd loading properly defined, but should contain core epi parts
define("epi/main", ["dojo"], function (dojo) {
    var epi = dojo.getObject("epi", true);
    /*=====
    var epi = {
        // summary:
        //      The main module.
        // tags:
        //      public
    };
    =====*/
    return epi;
});
