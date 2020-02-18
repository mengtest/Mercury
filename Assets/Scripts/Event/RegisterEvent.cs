using System;

public class RegisterEvent : EventArgs
{
    public readonly RegisterManager manager;

    public RegisterEvent(RegisterManager manager) { this.manager = manager; }

    public class Pre : RegisterEvent
    {
        public Pre(RegisterManager manager) : base(manager) { }
    }

    public class AfterAuto : RegisterEvent
    {
        public AfterAuto(RegisterManager manager) : base(manager) { }
    }
}