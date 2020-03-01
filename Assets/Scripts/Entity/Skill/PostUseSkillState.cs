using System;

namespace Mercury
{
    public class PostUseSkillState : WaitState
    {
        private readonly SkillSystemImpl _impl;
        
        public event EventHandler<EntitySkillEvent.PostUse> OnPostUseSkill;

        public PostUseSkillState(string id, FsmSystem system, SkillSystemImpl impl) : base(id, system) { _impl = impl; }

        public override void OnLeave()
        {
            base.OnLeave();
            _impl.UsingSkill.OnPostUse();
            OnPostUseSkill?.Invoke(this, new EntitySkillEvent.PostUse(_impl.UsingSkill));
            _impl.UsingSkill = null;
        }
    }
}