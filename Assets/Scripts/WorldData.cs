using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    public class WorldData : MonoBehaviour
    {
        private GameManager _gameManager;

        public List<Entity> ActiveEntities { get; private set; }

        private void Awake()
        {
            ActiveEntities = new List<Entity>();
            _gameManager = GameManager.Instance;
            _gameManager.SetActiveWorld(this);
        }

        public Entity SpawnEntity(AssetLocation id)
        {
            var entry = _gameManager.Registries.QueryRegistryEntry<EntityEntry>("entity", id);
            var e = entry?.SpawnEntity();
            if (e == null)
            {
                return null;
            }

            ActiveEntities.Add(e);
            return e;
        }
    }
}