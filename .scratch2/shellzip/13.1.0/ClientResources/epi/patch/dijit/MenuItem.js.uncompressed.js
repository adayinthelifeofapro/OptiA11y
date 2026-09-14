require({cache:{
'url:epi/patch/dijit/templates/MenuItem.html':"<tr class=\"dijitReset dijitMenuItem\" data-dojo-attach-point=\"focusNode\" role=\"menuitem\" tabIndex=\"-1\">\n    <td class=\"dijitReset dijitMenuItemIconCell\" role=\"presentation\">\n        <span class=\"dijitInline dijitIcon dijitMenuItemIcon\" data-dojo-attach-point=\"iconNode\"/>\n    </td>\n    <td class=\"dijitReset dijitMenuItemLabel\" colspan=\"2\" data-dojo-attach-point=\"containerNode\"></td>\n    <td class=\"dijitReset dijitMenuItemAccelKey\" style=\"display: none\" data-dojo-attach-point=\"accelKeyNode\"></td>\n    <td class=\"dijitReset dijitMenuArrowCell\" role=\"presentation\">\n        <div data-dojo-attach-point=\"arrowWrapper\" style=\"visibility: hidden\">\n            <span class=\"dijitMenuExpand\"/>\n            <span class=\"dijitMenuExpandA11y\">+</span>\n        </div>\n    </td>\n</tr>\n",
'url:epi/patch/dijit/templates/CheckedMenuItem.html':"<tr class=\"dijitReset dijitMenuItem\" data-dojo-attach-point=\"focusNode\" role=\"menuitemcheckbox\" tabIndex=\"-1\" aria-checked=\"${checked}\">\n    <td class=\"dijitReset dijitMenuItemIconCell\" role=\"presentation\">\n        <span class=\"dijitInline dijitIcon dijitMenuItemIcon\" data-dojo-attach-point=\"iconNode\" />\n    </td>\n    <td class=\"dijitReset dijitMenuItemLabel\" colspan=\"2\" data-dojo-attach-point=\"containerNode,labelNode\"></td>\n    <td class=\"dijitReset dijitMenuItemAccelKey\" style=\"display: none\" data-dojo-attach-point=\"accelKeyNode\"></td>\n    <td class=\"dijitReset dijitMenuArrowCell\" role=\"presentation\">&#160;</td>\n</tr>\n"}});
define("epi/patch/dijit/MenuItem", [
    "dijit/MenuItem",
    "dijit/CheckedMenuItem",
    "dojo/text!./templates/MenuItem.html",
    "dojo/text!./templates/CheckedMenuItem.html",
    "dojo/dom-attr"
], function (MenuItem, CheckedMenuItem, menuItemTemplate, checkedMenuItemTemplate, domAttr) {
    // Patch to change iconNode from img to span, so that Axiom icons can be used.
    Object.assign(MenuItem.prototype, {
        templateString: menuItemTemplate,
        _setTooltipAttr: function (val) {
            this._set("tooltip", val);
            if (val) {
                domAttr.set(this.domNode, "title", val);
            }
            else {
                domAttr.remove(this.domNode, "title");
            }
        }
    });
    Object.assign(CheckedMenuItem.prototype, {
        templateString: checkedMenuItemTemplate
    });
});
