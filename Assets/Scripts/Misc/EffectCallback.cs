using System;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 特效回调
    /// </summary>
    public class EffectCallback : MonoBehaviour
    {
        /// <summary>
        /// 用于动画的回调
        /// </summary>
        public event Action EventA;

        /// <summary>
        /// 动画触发的事件函数
        /// </summary>
        private void OnAnimationTriggerEventA() { EventA?.Invoke(); }
    }
}