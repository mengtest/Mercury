using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 正在使用技能状态
    /// </summary>
    public class UsingSkillState : FsmStateImpl
    {
        private readonly SkillSystemImpl _impl;

        public UsingSkillState(FsmSystem system, SkillSystemImpl impl) : base("using", system, new List<TransitionInfo>(2)) { _impl = impl; }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _impl.UpdateFrame();
        }

        public override void OnLeave()
        {
            base.OnLeave();
            _impl.WillLeaveUpdate();
        }
    }
}