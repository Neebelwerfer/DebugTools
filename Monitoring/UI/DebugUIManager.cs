using Monitoring.Tools;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Monitoring.UI
{
    public class DebugUIManager : MonoBehaviour
    {
        public Vector2 UIPosition = new Vector2(10, 10);
        public Vector2 UISize = new Vector2(300, 200);


        List<IUIMenu> UIMenuList = new();

        IUIMenu ActiveMenu;

        private void Awake()
        {
            UIMenuList.Add(new InstanceUI());
            UIMenuList.Add(new GlobalMethodsUI());
            UIMenuList.Add(new ToolUI());
        }

        private void OnGUI()
        {
            Rect size = new(UIPosition.x, UIPosition.y, UISize.x, UISize.y);

            if(Cursor.lockState == CursorLockMode.Locked && Input.GetKeyUp(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible= true;
            }

            if(Input.GetMouseButtonUp(0))
            {
                var mousePos = Event.current.mousePosition;
                if(!size.Contains(mousePos))
                {
                    if (ActiveMenu != null) ActiveMenu.OnClick(ClickType.Up, 0, false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                if (ActiveMenu != null) ActiveMenu.OnClick(ClickType.Up, 0, true);
            }


            GUI.Box(size, "Debug Menu");
            GUILayout.BeginArea(size);
            GUILayout.Space(20);
            if(ActiveMenu == null)
            {
                foreach (var menu in UIMenuList)
                {
                    if(!menu.HasData()) continue;
                    if (GUILayout.Button(menu.DisplayName()))
                        ActiveMenu = menu;
                }
            }
            else
            {
                ActiveMenu.OnGUI();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<<"))
                {
                    ActiveMenu.Exit();
                    ActiveMenu = null;
                }
                if (GUILayout.Button("<"))
                {
                    if(!ActiveMenu.Back())
                        ActiveMenu = null;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }
}
#endif