define("epi/shell/widget/ComponentChrome", [
    "dojo/_base/declare",
    "dijit/layout/BorderContainer",
    "epi/shell/widget/_FocusableMixin",
    "epi/shell/widget/command/CommandToolbar", // Used in the template.
    "epi/i18n!epi/shell/ui/nls/EPiServer.Shell.UI.Resources.GadgetChrome",
    "epi/shell/command/_CommandConsumerMixin",
    "epi/shell/command/builder/ButtonBuilder",
    "epi/shell/command/builder/DropDownButtonBuilder",
    "epi/shell/command/builder/MenuAssembler"
], function (declare, BorderContainer, _FocusableMixin, Toolbar, resources, _CommandConsumerMixin, ButtonBuilder, DropDownButtonBuilder, MenuAssembler) {
    return declare([BorderContainer, _CommandConsumerMixin, _FocusableMixin], {
        // tags:
        //      internal
        gutters: false,
        iconClass: "dijitNoIcon",
        postCreate: function () {
            this.inherited(arguments);
            //Create the toolbar and place it in the bottom region
            this.toolbar = new Toolbar({
                "class": "epi-componentToolbar",
                region: "bottom",
                groups: ["default", "context", "setting"]
            });
            this.inherited("addChild", [this.toolbar]);
            var itemSettings = { showLabel: false };
            var settings = Object.assign({ "class": "epi-componentToolbarButton epi-mediumButton" }, itemSettings);
            // Setup the command builders for the toolbar.
            var buttonBuilder = new ButtonBuilder({
                settings: settings,
                optionClass: "epi-menu--inverted",
                optionItemClass: "epi-radioMenuItem"
            });
            var contextBuilder = new DropDownButtonBuilder({
                itemSettings: itemSettings,
                settings: Object.assign({}, settings, {
                    iconClass: "epi-iconContextMenu",
                    title: resources.contextmenutooltip
                })
            });
            var settingBuilder = new DropDownButtonBuilder({
                itemSettings: itemSettings,
                settings: Object.assign({}, settings, {
                    "class": "epi-componentSettingsButton epi-componentToolbarButton epi-mediumButton",
                    iconClass: "epi-iconSettingsMenu",
                    title: resources.settingsmenutooltip
                })
            });
            var menuAssemblerConfiguration = [
                { builder: buttonBuilder, target: this.toolbar },
                { category: "setting", builder: settingBuilder, target: this.toolbar },
                { category: "context", builder: contextBuilder, target: this.toolbar },
                { category: "menuWithSeparator", builder: contextBuilder, target: this.toolbar },
                { category: "popup", builder: contextBuilder, target: this.toolbar }
            ];
            var menuAssembler = new MenuAssembler({
                configuration: menuAssemblerConfiguration,
                commandSource: this
            });
            menuAssembler.build();
        },
        addChild: function (child) {
            // summary:
            //		Overridden to propagate child title property to ourself.
            if (child) {
                this.set("heading", child.get("heading"));
            }
            //Set the region to center
            child.region = "center";
            this.inherited(arguments);
        }
    });
});
