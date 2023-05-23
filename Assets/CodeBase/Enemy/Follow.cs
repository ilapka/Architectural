using UnityEngine;

namespace Enemy
{
    public abstract class Follow : MonoBehaviour
    {
        public void SetEnable(bool enable)
        {
            enabled = enable;
        }
    }
}