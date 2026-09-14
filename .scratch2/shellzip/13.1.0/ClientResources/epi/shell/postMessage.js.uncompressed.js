define("epi/shell/postMessage", [
    "require",
    "dojo/Evented",
    "dojo/on"
], function (moduleRequire, Evented, on) {
    var hub = new Evented(), messageHandler, context;
    function getContext() {
        return context || window.top;
    }
    function setupListener() {
        messageHandler && messageHandler.remove();
        try {
            messageHandler = on(getContext(), "message", function (evt) {
                var data = evt.data;
                data && data.id && hub.emit.apply(hub, [data.id, data.message]);
            }, false);
        }
        catch (e) {
            console.warn("Cannot attach to 'message' event", e);
        }
    }
    var module = {
        // summary:
        //      Adds the possibility to post messages to other windows and frames.
        //
        // example:
        //      |   postMessage.subscribe("some/topic", function(data) {
        //      |   ... do something with event
        //      |   });
        //      |   postMessage.publish("some/topic", {name:"some data"});
        //
        // tags:
        //      internal
        setContext: function (/* window */ ctx) {
            // summary:
            //      Set to which window object messaging should take place.
            // ctx: Window/object
            //      An object implementing at least on, frames and postMessage
            context = ctx;
            setupListener();
        },
        publish: function (topic, message) {
            // summary:
            //      Publishes a message to a topic on the postMessage hub.
            // topic: String
            //      The name of the topic to publish to
            // message: Object
            //      An event to distribute to the topic listeners
            moduleRequire(["epi/shell/dist/messagePublisher"], function (messagePublisher) {
                messagePublisher.publish(topic, message);
            });
        },
        subscribe: function (topic, listener) {
            // summary:
            //      Subscribes to a topic on postMessage hub.
            // topic: String
            //      The topic to subscribe to
            // listener: Function
            //      A function to call when a message is published to the given topic
            return hub.on.apply(hub, arguments);
        }
    };
    setupListener();
    return module;
});
