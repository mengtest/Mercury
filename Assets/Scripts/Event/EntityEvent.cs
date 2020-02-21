using System;

public abstract class EntityEvent : EventArgs
{
    public readonly Entity entity;

    public class Awake : EntityEvent
    {
        public Awake(Entity entity) : base(entity) { }
    }

    public EntityEvent(Entity entity) { this.entity = entity; }

    public class Start : EntityEvent
    {
        public Start(Entity entity) : base(entity) { }
    }
}