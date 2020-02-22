using System;

public abstract class InjectEvent : EventArgs
{
    public class InitContainer : InjectEvent
    {
        public readonly IoCContainer container;
        public InitContainer(IoCContainer container) { this.container = container; }
    }
}