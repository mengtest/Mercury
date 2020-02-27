using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 等待状态
    /// </summary>
    public class WaitState : FsmStateImpl
    {
        /// <summary>
        /// 等待结束时间
        /// </summary>
        private float _endTime;

        /// <summary>
        /// 等待结束时间。对该属性赋值，会自动加上当前时间。因此直接赋予需要等待多少秒就可以了
        /// </summary>
        public float EndTime { get => _endTime; set => _endTime = value + Time.time; }

        public WaitState(string id, FsmSystem system) : base(id, system, new List<TransitionInfo>(1)) { }
    }
}