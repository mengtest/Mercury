using System;
using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 使用技能的状态
    /// </summary>
    public class UseSkillState : FsmStateImpl
    {
        public ISkill UsingSkill { get; set; }

        public event EventHandler<EntitySkillEvent.PreUse> OnPreUseSkill;
        public event EventHandler<EntitySkillEvent.Using> OnUsingSkill;

        public UseSkillState(string id, FsmSystem system) : base(id, system, new List<TransitionInfo>(1)) { }

        public override void OnEnter()
        {
            base.OnEnter();
            UsingSkill.OnPreUse();
            OnPreUseSkill?.Invoke(this, new EntitySkillEvent.PreUse(UsingSkill));
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UsingSkill.OnUsing();
            OnUsingSkill?.Invoke(this, new EntitySkillEvent.Using(UsingSkill));
        }
    }
}