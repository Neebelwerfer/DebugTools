using Monitoring.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Monitoring.Tools
{
    internal class GameTimeTool : IUIMenu
    {
        readonly float _DefaultTimeScale = 0f;
        float _TimeScale = 0f;

        public GameTimeTool()
        {
            _TimeScale = Time.timeScale;
            _DefaultTimeScale = _TimeScale;
        }  

        public bool Back()
        {
            return false;
        }

        public string DisplayName()
        {
            return "Game Time Tool";
        }

        public void Exit()
        {
            Time.timeScale = _DefaultTimeScale;
            _TimeScale = _DefaultTimeScale;
        }

        public bool HasData()
        {
            return true;
        }

        public void OnClick(ClickType type, int mouse, bool MenuClicked)
        {
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Scale: " + _TimeScale);
            _TimeScale = GUILayout.HorizontalSlider(_TimeScale, 0, 2);
            GUILayout.EndHorizontal();
            if (_TimeScale == 0f)
            {
                if (GUILayout.Button("Start"))
                    _TimeScale = _DefaultTimeScale;
            }
            else
            {
                if (GUILayout.Button("Pause"))
                    _TimeScale = 0f;
            }
            if (GUILayout.Button("Reset to default"))
                _TimeScale= _DefaultTimeScale;

            Time.timeScale= _TimeScale;
        }
    }
}
