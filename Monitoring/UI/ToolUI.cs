using Monitoring.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Monitoring.UI
{
    internal class ToolUI : IUIMenu
    {
        string Path = "";

        IUIMenu ActiveMenu = null;
        List<IUIMenu> MenuItems = new List<IUIMenu>();
        
        public ToolUI()
        {
            MenuItems.Add(new GameTimeTool());
            MenuItems.Add(new InstanceSelectorTool());
            MenuItems.Add(new DebugCameraTool());
        }

        public bool Back()
        {
            if (Path != "")
            {
                Path = DisplayUtility.RemoveLastPartOfPath(Path);
                return true;
            }
            if(ActiveMenu != null)
            {
                if(!ActiveMenu.Back())
                {
                    ActiveMenu = null;
                    return true;
                }
                return true;
            }
            return false;
        }

        public string DisplayName()
        {
            return "Debug Tools";
        }

        public void Exit()
        {
            Path = "";
        }

        public bool HasData()
        {
            return true;
        }

        public void OnClick(ClickType type, int mouse, bool MenuClicked)
        {
  
            if(ActiveMenu != null)ActiveMenu.OnClick(type, mouse, MenuClicked);
        }

        public void OnGUI()
        {
            if (ActiveMenu != null)
            {
                ActiveMenu.OnGUI();
            }
            else
            {
                foreach (var item in MenuItems)
                {
                    if (GUILayout.Button(item.DisplayName()))
                        ActiveMenu = item;
                }
            }
        }
    }
}
