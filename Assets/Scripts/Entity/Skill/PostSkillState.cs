namespace Mercury
{
    /// <summary>
    /// 后摇状态
    /// </summary>
    public class PostSkillState : WaitState
    {
        public PostSkillState(FsmSystem system) : base("post", system) { }
    }
}