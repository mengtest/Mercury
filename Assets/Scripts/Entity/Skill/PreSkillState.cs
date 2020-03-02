namespace Mercury
{
    public class PreSkillState : WaitState
    {
        private readonly SkillSystemImpl _impl;

        public PreSkillState(FsmSystem system, SkillSystemImpl impl) : base("pre", system) { _impl = impl; }

        public override void OnEnter()
        {
            base.OnEnter();
            _impl.EnterPre();
        }
    }
}