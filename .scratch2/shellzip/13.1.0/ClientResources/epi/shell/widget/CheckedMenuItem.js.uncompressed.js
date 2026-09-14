require({cache:{
'url:epi/shell/widget/templates/CheckedMenuItem.html':"﻿<tr class=\"dijitReset dijitMenuItem\" data-dojo-attach-point=\"focusNode\" role=\"menuitemcheckbox\" tabIndex=\"-1\">\n\t<td class=\"dijitReset dijitMenuItemIconCell\" role=\"presentation\">\n        <span class=\"dijitMenuItemIcon dijitCheckedMenuItemIcon\"></span>\n        <span class=\"dijitInline\" data-dojo-attach-point=\"iconNode\"></span>\n\t\t<span class=\"dijitCheckedMenuItemIconChar\">&#10003;</span>\n\t</td>\n\t<td class=\"dijitReset dijitMenuItemLabel\" colspan=\"2\" data-dojo-attach-point=\"containerNode,labelNode\"></td>\n\t<td class=\"dijitReset dijitMenuItemAccelKey\" style=\"display: none\" data-dojo-attach-point=\"accelKeyNode\"></td>\n\t<td class=\"dijitReset dijitMenuArrowCell\" role=\"presentation\">&#160;</td>\n</tr>\n"}});
define("epi/shell/widget/CheckedMenuItem", [
    "dojo/_base/declare",
    "dijit/CheckedMenuItem",
    "dojo/text!./templates/CheckedMenuItem.html"
], function (declare, CheckedMenuItem, template) {
    return declare(CheckedMenuItem, {
        // summary:
        //		A checkbox-like menu item for toggling on and off
        //
        // tags:
        //      internal xproduct
        // templateString: String
        //      The widget template string.
        templateString: template
    });
});
