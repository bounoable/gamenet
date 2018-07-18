using System;
using System.Collections.Generic;

namespace GameNet.Debug
{
    public class ClientDebugger: IClientDebugger
    {
        Dictionary<ClientEvent, HashSet<Action<EventData>>> events = new Dictionary<ClientEvent, HashSet<Action<EventData>>>();

        /// <summary>
        /// Register a handler for an event type.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="handler">The event handler.</param>
        /// <returns>The debugger instance.</returns>
        public ClientDebugger On(ClientEvent ev, Action<EventData> handler)
        {
            if (!events.TryGetValue(ev, out var handlers)) {
                handlers = new HashSet<Action<EventData>>();
                events[ev] = handlers;
            }

            handlers.Add(handler);

            return this;
        }

        public ClientDebugger On(ClientEvent ev, Action handler) => On(ev, data => handler());

        /// <summary>
        /// Handle an event.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="data">The event data.</param>
        public void Handle(ClientEvent ev, EventData data = null)
        {
            if (!events.TryGetValue(ev, out var handlers)) {
                return;
            }

            foreach (Action<EventData> handler in handlers) {
                handler(data);
            }
        }
    }
}