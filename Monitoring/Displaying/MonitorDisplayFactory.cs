using System;
using Monitoring.Attributes;

namespace Monitoring.Display
{
    public static class MonitorDisplayFactory
    {
        public static TrackedValueDrawer GetDefaultDrawer(Type value)
        {
            if (value == typeof(float)) return new DefaultFloatDisplay(2);
            if (value == typeof(bool)) return new DefaultBoolDisplay();
            else return new DefaultDisplay();
        }
    }
}