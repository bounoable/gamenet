using System;
using GameNet.Debug;
using System.Collections.Generic;

namespace GameNet.Tests.Debug
{
    class TestServerDebugger: ServerDebugger
    {
        HashSet<ServerEvent> triggeredEvents = new HashSet<ServerEvent>();

        public TestServerDebugger()
        {
            foreach (var ev in (ServerEvent[])Enum.GetValues(typeof(ServerEvent))) {
                On(ev, () => triggeredEvents.Add(ev));
            }
        }

        public bool Triggered(ServerEvent ev) => triggeredEvents.Contains(ev);
    }
}