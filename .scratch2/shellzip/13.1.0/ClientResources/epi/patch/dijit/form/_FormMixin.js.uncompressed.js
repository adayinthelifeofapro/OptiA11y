// Currently only a place holder for the core epi namespace
// Needed to get the amd loading properly defined, but should contain core epi parts
define("epi/patch/dijit/form/_FormMixin", [
    "dojo/_base/array",
    "dojo/_base/lang",
    "dijit/form/_FormMixin"
], function (array, lang, _FormMixin) {
    // module:
    //		dijit/patch/form/_FormMixin
    // summary:
    //		Fix issue with null array when setting value to form
    lang.mixin(_FormMixin.prototype, {
        _setValueAttr: function (/*Object*/ obj) {
            // summary:
            //		Fill in form values from according to an Object (in the format returned by get('value'))
            // generate map from name --> [list of widgets with that name]
            var map = {};
            array.forEach(this._getDescendantFormWidgets(), function (widget) {
                if (!widget.name) {
                    return;
                }
                var entry = map[widget.name] || (map[widget.name] = []);
                entry.push(widget);
            });
            for (var name in map) {
                if (!map.hasOwnProperty(name)) {
                    continue;
                }
                var widgets = map[name], // array of widgets w/this name
                values = lang.getObject(name, false, obj); // list of values for those widgets
                if (values === undefined) {
                    continue;
                }
                if (!lang.isArray(values)) {
                    /* THE FIX GOES HERE */
                    /* --------------------------------------------------------------------------------------------- */
                    // If widget expects an array and value is null, it shouldn't get [null] like the value which dijit._FormMixin sets.
                    if (values === null && widgets[0].multiple) {
                        values = null;
                    }
                    else {
                        values = [values];
                    }
                    /* --------------------------------------------------------------------------------------------- */
                    /* END FIX */
                }
                if (typeof widgets[0].checked == 'boolean') {
                    // for checkbox/radio, values is a list of which widgets should be checked
                    array.forEach(widgets, function (w) {
                        w.set('value', array.indexOf(values, w.value) !== -1);
                    });
                }
                else if (widgets[0].multiple) {
                    // it takes an array (e.g. multi-select)
                    widgets[0].set('value', values);
                }
                else {
                    // otherwise, values is a list of values to be assigned sequentially to each widget
                    array.forEach(widgets, function (w, i) {
                        w.set('value', values[i]);
                    });
                }
            }
        },
        _getValueAttr: function () {
            // summary:
            //		Returns Object representing form values.   See description of `value` for details.
            // description:
            // The value is updated into this.value every time a child has an onChange event,
            // so in the common case this function could just return this.value.   However,
            // that wouldn't work when:
            //
            // 1. User presses return key to submit a form.  That doesn't fire an onchange event,
            // and even if it did it would come too late due to the defer(...) in _handleOnChange()
            //
            // 2. app for some reason calls this.get("value") while the user is typing into a
            // form field.   Not sure if that case needs to be supported or not.
            // get widget values
            var obj = {};
            array.forEach(this._getDescendantFormWidgets(), function (widget) {
                var name = widget.name;
                if (!name || widget.disabled) {
                    return;
                }
                // Single value widget (checkbox, radio, or plain <input> type widget)
                var value = widget.get('value');
                // Store widget's value(s) as a scalar, except for checkboxes which are automatically arrays
                if (typeof widget.checked == 'boolean') {
                    if (/Radio/.test(widget.declaredClass)) {
                        // radio button
                        if (value !== false) {
                            lang.setObject(name, value, obj);
                        }
                        else {
                            // give radio widgets a default of null
                            value = lang.getObject(name, false, obj);
                            if (value === undefined) {
                                lang.setObject(name, null, obj);
                            }
                        }
                    }
                    else {
                        // checkbox/toggle button
                        var ary = lang.getObject(name, false, obj);
                        if (!ary) {
                            ary = [];
                            lang.setObject(name, ary, obj);
                        }
                        if (value !== false) {
                            ary.push(value);
                        }
                    }
                }
                else {
                    var prev = lang.getObject(name, false, obj);
                    if (typeof prev != "undefined") {
                        if (lang.isArray(prev)) {
                            prev.push(value);
                            /* THE FIX GOES HERE */
                            /* --------------------------------------------------------------------------------------------- */
                        }
                        else if (typeof prev === "function") {
                            lang.setObject(name, value, obj);
                            /* --------------------------------------------------------------------------------------------- */
                            /* END FIX */
                        }
                        else {
                            lang.setObject(name, [prev, value], obj);
                        }
                    }
                    else {
                        // unique name
                        lang.setObject(name, value, obj);
                    }
                }
            });
            /***
             * code for plain input boxes (see also domForm.formToObject, can we use that instead of this code?
             * but it doesn't understand [] notation, presumably)
            var obj = { };
            array.forEach(this.containerNode.elements, function(elm){
                if(!elm.name)	{
                    return;		// like "continue"
                }
                var namePath = elm.name.split(".");
                var myObj=obj;
                var name=namePath[namePath.length-1];
                for(var j=1,len2=namePath.length;j<len2;++j){
                    var nameIndex = null;
                    var p=namePath[j - 1];
                    var nameA=p.split("[");
                    if(nameA.length > 1){
                        if(typeof(myObj[nameA[0]]) == "undefined"){
                            myObj[nameA[0]]=[ ];
                        } // if
                        nameIndex=parseInt(nameA[1]);
                        if(typeof(myObj[nameA[0]][nameIndex]) == "undefined"){
                            myObj[nameA[0]][nameIndex] = { };
                        }
                    }else if(typeof(myObj[nameA[0]]) == "undefined"){
                        myObj[nameA[0]] = { }
                    } // if

                    if(nameA.length == 1){
                        myObj=myObj[nameA[0]];
                    }else{
                        myObj=myObj[nameA[0]][nameIndex];
                    } // if
                } // for

                if((elm.type != "select-multiple" && elm.type != "checkbox" && elm.type != "radio") || (elm.type == "radio" && elm.checked)){
                    if(name == name.split("[")[0]){
                        myObj[name]=elm.value;
                    }else{
                        // can not set value when there is no name
                    }
                }else if(elm.type == "checkbox" && elm.checked){
                    if(typeof(myObj[name]) == 'undefined'){
                        myObj[name]=[ ];
                    }
                    myObj[name].push(elm.value);
                }else if(elm.type == "select-multiple"){
                    if(typeof(myObj[name]) == 'undefined'){
                        myObj[name]=[ ];
                    }
                    for(var jdx=0,len3=elm.options.length; jdx<len3; ++jdx){
                        if(elm.options[jdx].selected){
                            myObj[name].push(elm.options[jdx].value);
                        }
                    }
                } // if
                name=undefined;
            }); // forEach
            ***/
            return obj;
        }
    });
    _FormMixin.prototype._setValueAttr.nom = "_setValueAttr";
});
