using System;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 特效回调
    /// </summary>
    public class EffectCallback : MonoBehaviour
    {
        public event Action EventA;

        private void OnAnimationTriggerEventA() { EventA?.Invoke(); }
    }
}