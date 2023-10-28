using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Monitoring.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    [Preserve]
    public class MonitorField : TrackedAttribute
    {
        public bool ReadOnly;
        public bool HideInBuild;

        public MonitorField(string Category = "", string Name = "", bool ReadOnly = true, bool HideInBuild = false)
        {
            this.Category = Category;
            this.ReadOnly = ReadOnly;
            this.Name = Name;
            this.HideInBuild = HideInBuild;
        }
    }
}