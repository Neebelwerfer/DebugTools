using Monitoring.Display;
using System.Collections.Generic;
using UnityEngine;

namespace Monitoring.UI
{
    internal class InstanceUI : IUIMenu
    {
        string MethodPath = "";
        string Returned = "";
        Vector2 Scroll = new Vector2(0, 0);
        Node SelectedNode= null;
        Dictionary<object, MonitorTree> _MonitorTree;

        public InstanceUI()
        {
            _MonitorTree= new Dictionary<object, MonitorTree>();
            MonitorHandler.AddedMonitoredObject += AddedTrackedBehavior;
            MonitorHandler.RemovedMonitoredObject += RemovedTrackedBehaviour;
            SetupTrackedValues();
        }
        private void RemovedTrackedBehaviour(object trackedClass, TrackedInstance trackedObject)
        {
            _MonitorTree.Remove(trackedClass);
        }

        private void AddedTrackedBehavior(object trackedClass, TrackedInstance trackedObject)
        {
            if (!_MonitorTree.ContainsKey(trackedClass))
            {
                _MonitorTree.Add(trackedClass, new MonitorTree(trackedClass, trackedObject.Values, trackedObject.Methods));
            }
        }

        void SetupTrackedValues()
        {
            foreach (var trackedObject in MonitorHandler.GetTrackedInstances())
            {
                var obj = trackedObject.Value;
                if (!_MonitorTree.ContainsKey(trackedObject))
                {
                    _MonitorTree.Add(trackedObject, new MonitorTree(trackedObject, obj.Values, obj.Methods));
                }
            }
        }

        void DisplayMethods(List<TrackedMethod> methods)
        {
            foreach (var method in methods)
            {
                if (method is null) continue;
                if (MethodPath == method.Options.Name)
                    DisplayMethod(method);
                else
                {
                    if (GUILayout.Button(method.Options.Name + "()"))
                        MethodPath = method.Options.Name;
                }
            }
        }

        void DisplayMethod(TrackedMethod method)
        {
            var res = DisplayUtility.DisplayMethod(method);
            if (res != "") Returned = res;
            if (Returned != "")
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Returned: ");
                GUILayout.Label(Returned);
                GUILayout.EndHorizontal();
            }
        }

        void DisplayValues(List<TrackedValue> values)
        {
            foreach (var value in values)
            {
                if (value is null) continue;
                if (MethodPath != "") continue;
                GUILayout.BeginHorizontal();
                value.DisplayDrawer?.OnDraw(value);
                GUILayout.EndHorizontal();
            }
        }

        void DisplayInstances()
        {
            if (SelectedNode == null)
            {
                foreach (var (instance, tree) in _MonitorTree)
                {
                    if (GUILayout.Button(instance.ToString()))
                    {
                        SelectedNode = tree.RootNode;
                    }
                }
            }
            else
            {
                Scroll = GUILayout.BeginScrollView(Scroll);
                
                if (MethodPath == "" && SelectedNode.Nodes != null && SelectedNode.Nodes.Count > 0)
                {
                    foreach (var c in SelectedNode.Nodes)
                    {
                        if (GUILayout.Button(c.PathName))
                        {
                            SelectedNode = c;
                            break;
                        }
                    }
                }

                if (SelectedNode.Values != null && SelectedNode.Values.Count > 0)
                {
                    DisplayValues(SelectedNode.Values);
                }

                if (SelectedNode.Methods != null && SelectedNode.Methods.Count > 0)
                {
                    DisplayMethods(SelectedNode.Methods);
                }
                GUILayout.EndScrollView();
            }
        }

        public void OnGUI()
        {
            if(_MonitorTree.Count == 0) return;
            DisplayInstances();

        }

        public bool Back()
        {
            Returned = "";
            if (MethodPath != "")
            {
                MethodPath = DisplayUtility.RemoveLastPartOfPath(MethodPath);
                return true;
            }
            if (SelectedNode == null) return false;
            SelectedNode = SelectedNode.Parent;
            return true;
        }

        public bool HasData()
        {
            return _MonitorTree.Count > 0;
        }

        public void Exit()
        {
            SelectedNode = null;
        }

        public string DisplayName()
        {
            return "Monitored Instances";
        }

        public void OnClick(ClickType type, int mouse, bool MenuClicked)
        {
        }
    }
}
