using UnityEngine;

namespace Monitoring
{
    public class MonitoredMonoBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
            MonitorHandler.AddBehaviour(this);
        }

        protected void OnDestroy()
        {
            MonitorHandler.RemoveBehaviour(this);
        }
    }
}

