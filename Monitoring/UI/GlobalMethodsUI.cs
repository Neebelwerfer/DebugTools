using UnityEngine;

namespace Monitoring.UI
{
    internal class GlobalMethodsUI : IUIMenu
    {
        string MethodPath = "";
        Vector2 Scroll = Vector2.zero;

        public bool Back()
        {
            if (MethodPath != "")
            {
                MethodPath = "";
                return true;
            }
            return false;
        }

        public void Exit()
        {
            MethodPath = "";
        }

        public bool HasData()
        {
            return MonitorHandler.GetGlobalMethods().Count > 0;
        }

        void DisplayGlobalMethods()
        {
            Scroll = GUILayout.BeginScrollView(Scroll);
            var methods = MonitorHandler.GetGlobalMethods();
            if (MethodPath == "")
            {
                foreach (var method in methods)
                {
                    if (GUILayout.Button(method.Options.Name))
                    {
                        MethodPath = method.Options.Name;
                    }
                }
            }
            else
            {
                DisplayUtility.DisplayMethod(methods.Find(m => m.Options.Name == MethodPath));

            }
            GUILayout.EndScrollView();
        }

        public void OnGUI()
        {
            DisplayGlobalMethods();
        }

        public string DisplayName()
        {
            return "Global Methods";
        }

        public void OnClick(ClickType type, int mouse, bool MenuClicked)
        {
        }
    }
}
