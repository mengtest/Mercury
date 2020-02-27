using UnityEngine;

namespace Mercury
{
    public class EntityReference : MonoBehaviour, IEntityComponent
    {
        public Entity Entity { get; set; }
        public string Id { get; } = "ref";
    }
}