using Monitoring.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Monitoring.Tools
{
    internal class InstanceSelectorTool : IUIMenu
    {
        public InstanceSelectorTool() { }

        public List<GameObject> SceneRootObject = new();
        public GameObject SelectedObject = null;

        public List<MonoBehaviour> ComponentCache = new();
        public GameObject ComponentCacheObject = null;

        Vector2 Scroll = Vector2.zero;
        bool CollapseChildObjects = true; 
        bool CollapseComponents = true;

        MonoBehaviour SelectedInstance = null;
        FieldInfo[] bindings = null;

        Camera cam = null;

        public bool Back()
        {
            if (SelectedInstance != null)
            {
                SelectedInstance = null;
                bindings = null;
                return true;
            }
            else if (SelectedObject != null)
            {
                var parent = SelectedObject.transform.parent;
                if(parent != null)
                    SelectedObject = parent.gameObject;
                else SelectedObject = null;
                return true;
            } 
            else if (SceneRootObject.Count > 0)
            {
                SceneRootObject = new List<GameObject>();
                return true;
            }
            Exit();
            return false;
        }

        public string DisplayName()
        {
            return "Select Scene Instance";
        }

        public void Exit()
        {
            ComponentCacheObject = new();
            Scroll = Vector2.zero;
            SceneRootObject = new List<GameObject>();
            SelectedObject = null;
        }

        public bool HasData()
        {
            return true;
        }

        public void OnGUI()
        {
            Scroll = GUILayout.BeginScrollView(Scroll);
            if(SceneRootObject.Count != 0 || SelectedObject != null)
            {
                if(SelectedObject == null)
                {
                    foreach (GameObject go in SceneRootObject)
                    {
                        if (GUILayout.Button(go.name))
                        {
                            SelectedObject = go;
                            break;
                        }
                    }
                } 
                else if (SelectedInstance == null) 
                {
                    DisplayUtility.DisplayGameObject(SelectedObject);
                    if (GUILayout.Button("Destroy"))
                    {
                        UnityEngine.Object.Destroy(SelectedObject.gameObject);
                        SelectedObject = null;
                        
                    }

                    if (ComponentCache == null || SelectedObject != ComponentCacheObject)
                    {
                        SelectedObject.GetComponents(ComponentCache);
                        ComponentCacheObject = SelectedObject;
                    }

                    if(ComponentCache != null || ComponentCache.Count != 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Components: " + ComponentCache.Count);
                        CollapseComponents = GUILayout.Toggle(CollapseComponents, "Collapse");
                        GUILayout.EndHorizontal();

                        if(!CollapseComponents)
                        {
                            foreach (MonoBehaviour mb in ComponentCache)
                            {
                                if (GUILayout.Button(mb.GetType().Name))
                                {
                                    SelectedInstance = mb;
                                    bindings = DisplayUtility.GetFieldInfos(mb.GetType());
                                }
                            }
                        }
                    }

                    if (SelectedObject.transform.childCount != 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child Objects: " + SelectedObject.transform.childCount);
                        CollapseChildObjects = GUILayout.Toggle(CollapseChildObjects, "Collapse");
                        GUILayout.EndHorizontal();

                        if (!CollapseChildObjects)
                        {
                            for (int i = 0; i < SelectedObject.transform.childCount; i++)
                            {
                                var child = SelectedObject.transform.GetChild(i);
                                if (GUILayout.Button(child.name))
                                {
                                    SelectedObject = child.gameObject;
                                }
                            }
                        }
                    } 
                 }
                else
                {
                    GUILayout.Label("Component: " + SelectedInstance.GetType().Name);
                    if(bindings.Length != 0)
                    {
                        foreach (var b in bindings)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(b.Name);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(b.GetValue(SelectedInstance).ToString());
                            GUILayout.EndHorizontal();
                        }

                        if (GUILayout.Button(SelectedInstance.enabled ? "Disable" : "Enable"))
                        {
                            SelectedInstance.enabled = !SelectedInstance.enabled;
                        }
                    }
                }
            } 
            else
            {
                if(GUILayout.Button("Find Objects"))
                {
                    var scene = SceneManager.GetActiveScene();
                    scene.GetRootGameObjects(SceneRootObject);
                }
            }

            GUILayout.EndScrollView();
        }

        public void OnClick(ClickType type, int mouse, bool MenuClicked)
        {
            if (SelectedInstance == null && !MenuClicked)
            {
                if (cam == null || !cam.enabled) cam = Camera.main;
                if (type == ClickType.Up && mouse == 0)
                {
                    var mousePos = Event.current.mousePosition;
                    mousePos.y = cam.pixelHeight - mousePos.y;
                    var point = cam.ScreenPointToRay(mousePos);
                    if (Physics.Raycast(point, out var hit))
                    {
                        SelectedObject = hit.transform.gameObject;
                    }
                }
            }
        }
    }
}
