using UnityEngine;

namespace Mercury
{
    public class UnityObject<T> : IEntityComponent where T : Object
    {
        public T Value { get; }
        public string Id { get; }

        public UnityObject(string id, T value)
        {
            Value = value;
            Id = id;
        }
    }
}