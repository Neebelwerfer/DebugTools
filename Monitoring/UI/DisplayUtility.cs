using Monitoring.Attributes;
using Monitoring.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Monitoring.UI
{
    internal static class DisplayUtility
    {
        internal static string RemoveLastPartOfPath(string path)
        {
            if (!path.Contains('/'))
            {
                return "";
            }
            else
            {
                var pathSegments = path.Split('/');
                var newPath = pathSegments[0];
                for (int i = 1; i < pathSegments.Length - 1; i++)
                {
                    newPath += "/" + pathSegments[i];
                }
                return newPath;
            }
        }

        internal static string DisplayMethod(TrackedMethod method)
        {
            string returnedData = "";
            var ActionInfo = (DebugAction)method.Options;
            GUILayout.Label(ActionInfo.Name);
            if (ActionInfo.Description != "") GUILayout.Label(ActionInfo.Description);
            if (method.Parameters == null || method.Parameters.Length == 0)
            {
                if (GUILayout.Button("Invoke"))
                {
                    if (method.CanReturn)
                    {
                        returnedData = method.Method.Invoke(method.Instance, null).ToString();
                    }
                    method.Method.Invoke(method.Instance, null);
                }
            }
            else
            {
                foreach (BaseParameter parameter in method.Parameters)
                {
                    GUILayout.BeginHorizontal();
                    parameter.OnDraw();
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Invoke"))
                {
                    if (method.CanReturn)
                    {
                        returnedData = method.Method.Invoke(method.Instance, method.Parameters.Select(x => x.Value).ToArray()).ToString();
                    }
                    method.Method.Invoke(method.Instance, method.Parameters.Select(x => x.Value).ToArray());
                }
            }
            return returnedData;
        }

        internal static FieldInfo[] GetFieldInfos(Type t)
        {
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<FieldInfo> proccessedList = new List<FieldInfo>();
            foreach (FieldInfo field in fields)
            {
                if (field.IsPublic)
                {
                    proccessedList.Add(field);
                } else if (field.IsPrivate)
                {
                    if(field.GetCustomAttribute<SerializeField>() != null) 
                        proccessedList.Add(field);
                }
            }
            return proccessedList.ToArray();
        }

        internal static void DisplayGameObject(GameObject go)
        {
            GUILayout.Label("GameObject: " + go.name);
            GUILayout.Label("Position: " + go.transform.position);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tag: " + go.tag);
            GUILayout.Label("Layer: " + LayerMask.LayerToName(go.layer));
            GUILayout.EndHorizontal();
            if (GUILayout.Button(go.activeInHierarchy ? "Disable" : "Enable"))
                go.SetActive(!go.activeInHierarchy);
        }

    }
}
