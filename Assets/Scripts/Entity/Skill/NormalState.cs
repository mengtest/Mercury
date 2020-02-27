using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 普通状态
    /// </summary>
    public class NormalState : FsmStateImpl
    {
        public NormalState(string id, FsmSystem system) : base(id, system, new List<TransitionInfo>(1)) { }
    }
}