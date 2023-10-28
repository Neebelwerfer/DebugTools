using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Monitoring.Display;
using Monitoring.Attributes;

namespace Monitoring
{
    public class TrackedObject
    {
        public object Instance;
        public TrackedAttribute Options;

    }
    public class TrackedValue : TrackedObject
    {
        public TrackedValueDrawer DisplayDrawer;
    }
    public class TrackedField : TrackedValue
    {
        public FieldInfo Field;

        public TrackedField(FieldInfo field, MonitorField debug, object instance, TrackedValueDrawer DisplayDrawer)
        {
            Field = field;
            Options = debug;
            Instance = instance;
            base.DisplayDrawer = DisplayDrawer;
        }
    }

    public class TrackedProperty : TrackedValue
    {
        public PropertyInfo Property;

        public TrackedProperty(PropertyInfo property, MonitorField debug, object classType, TrackedValueDrawer DisplayDrawer)
        {
            Property = property;
            Options = debug;
            Instance = classType;
            base.DisplayDrawer = DisplayDrawer;
        }
    }

    public class TrackedMethod : TrackedObject
    {
        public MethodInfo Method;
        public BaseParameter[] Parameters;
        public bool CanReturn;

        public TrackedMethod(object instance, MethodInfo method, BaseParameter[] parameters, DebugAction actionInfo, bool canReturn = false)
        {
            Instance = instance;
            Method = method;
            Parameters = parameters;
            Options = actionInfo;
            CanReturn = canReturn;
        }
    }

    public class TrackedInstance
    {
        public TrackedValue[] Values;
        public TrackedMethod[] Methods;
        public Type Class;
        public string Name;
    }

    public static class MonitorHandler
    {
        static readonly Dictionary<object, TrackedInstance> trackedInstances = new();
        static readonly List<TrackedMethod> GlobalTrackedMethods = new();

        public delegate void MonitorEvent(object trackedClass, TrackedInstance trackedObject);
        public static event MonitorEvent AddedMonitoredObject;
        public static event MonitorEvent RemovedMonitoredObject;

        public static Dictionary<object, TrackedInstance> GetTrackedInstances() => trackedInstances;
        public static List<TrackedMethod> GetGlobalMethods() => GlobalTrackedMethods;

        static void FindFields(object monitored, List<TrackedValue> trackedValues)
        {
            var fields = monitored.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var debug = field.GetCustomAttributes<MonitorField>();
                var option = field.GetCustomAttribute<TrackedValueDrawer>();
                if (debug.Count() == 0) continue;
                if (option == null) option = MonitorDisplayFactory.GetDefaultDrawer(field.FieldType);

                foreach (var f in debug)
                {
                    if (f.Name == "") f.Name = field.Name;
                    if (f.HideInBuild && !Application.isEditor) continue;
                    trackedValues.Add(new TrackedField(field, f, monitored, option));
                }
            }
        }

        static void FindProperties(object monitored, List<TrackedValue> trackedValues)
        {
            var properties = monitored.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var debug = prop.GetCustomAttributes<MonitorField>();
                var option = prop.GetCustomAttribute<TrackedValueDrawer>();
                if (debug.Count() == 0) continue;
                if (option == null) option = MonitorDisplayFactory.GetDefaultDrawer(prop.PropertyType);

                foreach (var p in debug)
                {
                    if (p.Name == "") p.Name = prop.Name;
                    trackedValues.Add(new TrackedProperty(prop, p, monitored, option));
                }
            }
        }

        static bool IsValid(MethodInfo method)
        {
            var paramList = method.GetParameters();
            if (paramList.Length == 0) return true;

            foreach (var param in paramList)
            {
                if (!param.ParameterType.IsPrimitive)
                {
                    if (param.ParameterType != typeof(string) && param.ParameterType != typeof(Vector3) && param.ParameterType != typeof(Vector2))
                        return false;
                }
            }
            return true;
        }

        static void FindMethods(object monitored, List<TrackedMethod> trackedMethods)
        {
            var methods = monitored.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            foreach (var m in methods)
            {
                var action = m.GetCustomAttribute<DebugAction>();
                if (action is null || !IsValid(m)) continue;
                if (action.Name == "") action.Name = m.Name;
                if (action.Global || m.IsStatic) GlobalTrackedMethods.Add(new TrackedMethod(monitored, m, ParameterFactory.ProcessParameters(m.GetParameters()), action, m.ReturnType != typeof(void)));
                else trackedMethods.Add(new TrackedMethod(monitored, m, ParameterFactory.ProcessParameters(m.GetParameters()), action, m.ReturnType != typeof(void)));
            }
        }

        public static void AddBehaviour(object monitored)
        {
            List<TrackedValue> trackedValues = new();
            List<TrackedMethod> trackedMethods = new();

            FindFields(monitored, trackedValues);
            FindProperties(monitored, trackedValues);
            FindMethods(monitored, trackedMethods);

            if (trackedValues.Count > 0 || trackedMethods.Count > 0)
            {
                TrackedInstance tracked = new();
                tracked.Values = trackedValues.ToArray();
                tracked.Methods = trackedMethods.ToArray();
                tracked.Class = monitored.GetType();
                
                if (trackedInstances.TryAdd(monitored, tracked))
                {
                    AddedMonitoredObject?.Invoke(monitored, tracked);
                }
                else
                {
                    Debug.LogError("Couldnt create Tracked Object");
                }
            }
        }

        public static void SetValue(TrackedValue trackedValue, object value)
        {
            if (trackedValue is TrackedField tf)
            {
                tf.Field.SetValue(trackedValue.Instance, value);
            }
            else if (trackedValue is TrackedProperty tp)
                tp.Property.SetValue(trackedValue.Instance, value);
        }

        public static object GetValue(TrackedValue trackedValue)
        {
            if (trackedValue is TrackedField tf)
                return tf.Field.GetValue(trackedValue.Instance);
            else if(trackedValue is TrackedProperty tp)
                return tp.Property.GetValue(trackedValue.Instance);
            return null;
        }

        public static void RemoveBehaviour(object monitored)
        {
            if (trackedInstances.ContainsKey(monitored))
            {
                RemovedMonitoredObject?.Invoke(monitored, trackedInstances.GetValueOrDefault(monitored));
                trackedInstances.Remove(monitored);
            }

            GlobalTrackedMethods.RemoveAll(method => method.Instance == monitored);
        }
    }
}