using UnityEngine;

namespace Mercury
{
    public class UnityObject<T> : IEntityComponent where T : Component
    {
        public T Value { get; }

        public UnityObject(T value) { Value = value; }
    }
}