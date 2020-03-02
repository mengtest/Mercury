using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 普通状态
    /// </summary>
    public class NormalState : FsmStateImpl
    {
        private readonly SkillSystemImpl _impl;

        public NormalState(FsmSystem system, SkillSystemImpl impl) : base("normalState", system, new List<TransitionInfo>(1)) { _impl = impl; }

        public override void OnEnter()
        {
            base.OnEnter();
            _impl.LeavePost();
        }
    }
}