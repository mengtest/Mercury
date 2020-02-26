using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    public class WaitState : FsmStateImpl
    {
        private float _endTime;

        public float EndTime { get => _endTime; set => _endTime = value + Time.time; }

        public WaitState(string id, FsmSystem system) : base(id, system, new List<TransitionInfo>(1)) { }
    }
}