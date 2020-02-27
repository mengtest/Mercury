using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 跟随目标的特效的回调
    /// </summary>
    public class FollowTargetEffectCallback : EffectCallback
    {
        public Transform target;
        public Vector3 offset;

        private void Update() { transform.position = target.position + offset; }
    }
}