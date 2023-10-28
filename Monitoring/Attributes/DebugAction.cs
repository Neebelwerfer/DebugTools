using System;
using UnityEngine.Scripting;

namespace Monitoring.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Preserve]
    public class DebugAction : TrackedAttribute
    {
        public bool Global;
        public string Description;

        public DebugAction(string name = "", string category = "", string description = "", bool global = false)
        {
            Name = name;
            Category = category;
            Global = global;
            Description = description;
        }
    }
}