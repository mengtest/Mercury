using System.Collections.Generic;

namespace Mercury
{
    public class UseSkillState : FsmStateImpl
    {
        public ISkill UsingSkill { get; set; }

        public UseSkillState(string id, FsmSystem system) : base(id, system, new List<TransitionInfo>(1)) { }

        public override void OnEnter()
        {
            base.OnEnter();
            UsingSkill.OnPreUse();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UsingSkill.OnUsing();
        }

        public override void OnLeave()
        {
            base.OnLeave();
            UsingSkill.OnPostUse();
            UsingSkill = null;
        }
    }
}