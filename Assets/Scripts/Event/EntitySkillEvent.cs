using System;

namespace Mercury
{
    public abstract class EntitySkillEvent : EventArgs
    {
        public class PreUse : EntitySkillEvent
        {
            public readonly ISkill willUsed;
            public PreUse(ISkill willUsed) { this.willUsed = willUsed; }
        }

        public class Using : EntitySkillEvent
        {
            public readonly ISkill usingSkill;
            public Using(ISkill usingSkill) { this.usingSkill = usingSkill; }
        }

        public class PostUse : EntitySkillEvent
        {
            public readonly ISkill used;
            public PostUse(ISkill used) { this.used = used; }
        }
    }
}