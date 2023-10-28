using Monitoring.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Monitoring.Tools
{
    class DebugCameraController : MonoBehaviour
    {
        public float speed = 0.5f;
        public float rotSpeed = 1f;
        Vector3 lastMousePos;

        private void Awake()
        {
            lastMousePos = Input.mousePosition;
        }

        private void LateUpdate()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * speed;
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * speed;
            }

            if(Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * speed;
            }

            if(Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * speed;
            }

            if (Cursor.lockState != CursorLockMode.Locked) return;
            var rot = transform.eulerAngles;
            rot.y += rotSpeed * MathF.Sign(Input.GetAxis("Mouse X"));
            rot.x -= rotSpeed * MathF.Sign(Input.GetAxis("Mouse Y"));
            rot.x = ClampAngle(rot.x, -70f, 70f);
            transform.eulerAngles = rot;
            lastMousePos = Input.mousePosition;
        }

        float ClampAngle(float current, float min, float max)
        {
            float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
            float hdtAngle = dtAngle * 0.5f;
            float midAngle = min + hdtAngle;

            float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
            if (offset > 0)
                current = Mathf.MoveTowardsAngle(current, midAngle, offset);
            return current;
        }

    }

    internal class DebugCameraTool : IUIMenu
    {
        float _DefaultTimeScale;
        Camera _MainCam = null;
        Camera DebugCam = null;
        DebugCameraController controller = null;

        public DebugCameraTool()
        {
            _DefaultTimeScale = Time.timeScale;
            if(_MainCam == null) _MainCam = Camera.main;
        }
        public bool Back()
        {
            return false;
        }

        public string DisplayName()
        {
            return "Debug Camera";
        }

        public void Exit()
        {
            if(DebugCam != null)UnityEngine.Object.Destroy(DebugCam.gameObject);
            controller = null;
            _MainCam.enabled = true;
            Time.timeScale = _DefaultTimeScale;
        }

        public bool HasData()
        {
            return true;
        }

        public bool OnClick(bool MenuClicked)
        {
            return false;
        }

        public void OnClick(ClickType type, int mouse, bool MenuClicked)
        {
        }

        public void OnGUI()
        {
            if(DebugCam != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Camera MoveSpeed: " + controller.speed);
                controller.speed = GUILayout.HorizontalSlider(controller.speed, 0, 1);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Camera CamSpeed: " + controller.rotSpeed);
                controller.rotSpeed = GUILayout.HorizontalSlider(controller.rotSpeed, 0, 1);
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Deactive"))
                {
                    UnityEngine.Object.Destroy(DebugCam.gameObject);
                    _MainCam.enabled = true;
                    Time.timeScale = _DefaultTimeScale;
                    controller = null;
                }
            }
            else
            {
                if (GUILayout.Button("Activate"))
                {
                    _MainCam.enabled = false;
                    DebugCam = new GameObject("Debug Cam").AddComponent<Camera>();
                    controller = DebugCam.gameObject.AddComponent<DebugCameraController>();
                    DebugCam.tag = "MainCamera";
                    DebugCam.transform.rotation = _MainCam.transform.rotation;
                    DebugCam.transform.position = _MainCam.transform.position;
                    Time.timeScale = 0;
                }
            }

        }
    }
}
