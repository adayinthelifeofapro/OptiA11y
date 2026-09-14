require({cache:{
'url:epi/shell/widget/templates/_LabeledInput_Top.htm':"﻿<div>\n\t<div class=\"editor-label\" dojoAttachPoint=\"labelContainer\">\n\t\t<label dojoAttachPoint=\"label\"></label>\n\t</div>\n\t<div class=\"editor-field\" dojoAttachPoint=\"inputContainer\">\n\t\t<div dojoAttachPoint=\"input\"></div>\n\t</div>\n</div>\n",
'url:epi/shell/widget/templates/_LabeledInput_Bottom.htm':"﻿<div>\n\t<div class=\"editor-field\" dojoAttachPoint=\"inputContainer\">\n\t\t<div dojoAttachPoint=\"input\"></div>\n\t</div>\n\t<div class=\"editor-label\" dojoAttachPoint=\"labelContainer\">\n\t\t<label dojoAttachPoint=\"label\"></label>\n\t</div>\n</div>\n",
'url:epi/shell/widget/templates/_LabeledInput_Left.htm':"﻿<div>\n\t<div class=\"editor-label\" style=\"float:left;\" dojoAttachPoint=\"labelContainer\">\n\t\t<label dojoAttachPoint=\"label\"></label>\n\t</div>\n\t<div class=\"editor-field\" dojoAttachPoint=\"inputContainer\">\n\t\t<div dojoAttachPoint=\"input\"></div>\n\t</div>\n\t<div style=\"clear: both;\"></div>\n</div>\n",
'url:epi/shell/widget/templates/_LabeledInput_Right.htm':"﻿<div>\n\t<div class=\"editor-label\" style=\"float: right;\" dojoAttachPoint=\"labelContainer\">\n\t\t<label dojoAttachPoint=\"label\"></label>\n\t</div>\n\t<div class=\"editor-field\" dojoAttachPoint=\"inputContainer\">\n\t\t<div dojoAttachPoint=\"input\"></div>\n\t</div>\n\t<div style=\"clear: both;\"></div>\n</div>\n"}});
define("epi/shell/widget/LabeledInput", [
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/dom-attr",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dojo/text!./templates/_LabeledInput_Top.htm",
    "dojo/text!./templates/_LabeledInput_Bottom.htm",
    "dojo/text!./templates/_LabeledInput_Left.htm",
    "dojo/text!./templates/_LabeledInput_Right.htm"
], function (declare, lang, domAttr, _Widget, _TemplatedMixin, topTemplate, bottomTemplate, leftTemplate, rightTemplate) {
    return declare([_Widget, _TemplatedMixin], {
        // summary:
        //      A widget which wrap another widget and create a label for it.
        //
        // tags:
        //      public
        // templateString: [protected] String
        //		A string that represents the widget template.
        templateString: null,
        postMixInProperties: function () {
            // summary:
            //		Create label, wrapped widget, and tight them up together
            var labelPos = this.params.labelPos;
            switch (labelPos) {
                case 2: // Bottom
                    this.templateString = bottomTemplate;
                    break;
                case 3: // Left
                    this.templateString = leftTemplate;
                    break;
                case 4: // Right
                    this.templateString = rightTemplate;
                    break;
                //case 1:
                default: // Top
                    this.templateString = topTemplate;
                    break;
            }
        },
        postCreate: function () {
            // summary:
            //		Create label, wrapped widget, and tight them up together
            //settings passed to constructor
            var settings = this.params;
            if (settings.wrappedDojoType) {
                //extract label information
                var wrappedDojoType = settings.wrappedDojoType;
                var labelText = settings.label;
                //don't send not neccessary settings to the wrapped widget
                delete settings.wrappedDojoType;
                delete settings.labelPos;
                //create widget to be wrapped
                require([wrappedDojoType], lang.hitch(this, function (ctor) {
                    var wrappedWidget = new ctor(settings, this.input);
                    //point the label to the right element
                    domAttr.set(this.label, "for", wrappedWidget.id);
                }));
                this.label.innerHTML = labelText;
            }
        }
    });
});
