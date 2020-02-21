using System;

public abstract class RegisterEvent : EventArgs
{
    public readonly RegisterManager manager;

    public RegisterEvent(RegisterManager manager) { this.manager = manager; }

    /// <summary>
    /// 只能在RegisterManager初始化之前订阅
    /// </summary>
    public class Pre : RegisterEvent
    {
        public Pre(RegisterManager manager) : base(manager) { }
    }

    /// <summary>
    /// 只能在RegisterManager初始化之前订阅
    /// </summary>
    public class AfterAuto : RegisterEvent
    {
        public AfterAuto(RegisterManager manager) : base(manager) { }
    }
}