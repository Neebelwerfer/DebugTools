using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Monitoring.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public abstract class TrackedValueDrawer : Attribute
    {
        protected void DefaultLabel(TrackedValue value)
        {
            GUILayout.Label(value.Options.Name);
            GUILayout.FlexibleSpace();
        }

        protected object GetValue(TrackedValue value)
        {
            return MonitorHandler.GetValue(value);
        }

        protected void SetValue(TrackedValue value, object valueToSet)
        {
            MonitorHandler.SetValue(value, valueToSet);
        }

        public virtual void OnDraw(TrackedValue value)
        {
            DefaultLabel(value);
        }
    }

    public class DefaultDisplay : TrackedValueDrawer
    {
        public override void OnDraw(TrackedValue value)
        {
            base.OnDraw(value);
            var res = MonitorHandler.GetValue(value);
            GUILayout.Label(res.ToString());
        }
    }

    public class DefaultFloatDisplay : TrackedValueDrawer
    {
        public int AllowedDecimals = 2;
        public DefaultFloatDisplay(int allowedDecimals = 2)
        {
            AllowedDecimals = allowedDecimals;
        }

        public override void OnDraw(TrackedValue value)
        {
            base.OnDraw(value);
            var f = (float)GetValue(value);
            GUILayout.Label(MathF.Round(f, AllowedDecimals).ToString());
        }
    }

    public class DefaultBoolDisplay : TrackedValueDrawer
    {

        public override void OnDraw(TrackedValue value)
        {
            base.OnDraw(value);
            var b = (bool)GetValue(value);
            var op = (MonitorField)value.Options;
            if(op.ReadOnly)
            {
                GUILayout.Label(b.ToString());
            }
            else
            {
                bool newB = GUILayout.Toggle(b, b.ToString());
                if(newB != b) 
                    SetValue(value, newB);
            }
        }
    }
}
