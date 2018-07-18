using System;
using System.Collections.Generic;

namespace GameNet.Debug
{
    public class ServerDebugger: IServerDebugger
    {
        Dictionary<ServerEvent, HashSet<Action<EventData>>> events = new Dictionary<ServerEvent, HashSet<Action<EventData>>>();

        /// <summary>
        /// Register a handler for an event type.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="handler">The event handler.</param>
        /// <returns>The debugger instance.</returns>
        public ServerDebugger On(ServerEvent ev, Action<EventData> handler)
        {
            if (!events.TryGetValue(ev, out var handlers)) {
                handlers = new HashSet<Action<EventData>>();
                events[ev] = handlers;
            }

            handlers.Add(handler);

            return this;
        }

        public ServerDebugger On(ServerEvent ev, Action handler) => On(ev, data => handler());

        /// <summary>
        /// Handle an event.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="data">The event data.</param>
        public void Handle(ServerEvent ev, EventData data = null)
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