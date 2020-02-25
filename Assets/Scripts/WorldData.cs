using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 世界数据，在每个场景都应该有一个
    /// </summary>
    public class WorldData : MonoBehaviour
    {
        /// <summary>
        /// 需要的资源是否加载
        /// </summary>
        public bool isAssetLoaded;

        private GameManager _gameManager;

        /// <summary>
        /// 需要每帧更新的Entity
        /// </summary>
        private List<IUpdatable> _updateEntities;

        /// <summary>
        /// 活动中的Entity
        /// </summary>
        public List<Entity> ActiveEntities { get; private set; }

        private void Awake()
        {
            ActiveEntities = new List<Entity>();
            _updateEntities = new List<IUpdatable>();
            _gameManager = GameManager.Instance;
            _gameManager.SetActiveWorld(this);
        }

        private void Update()
        {
            switch (_gameManager.nowState)
            {
                case GameState.Running:
                {
                    foreach (var updateEntity in _updateEntities)
                    {
                        updateEntity.OnUpdate(); //TODO:Job并行的可行性？
                    }

                    break;
                }
                case GameState.Waiting when !isAssetLoaded:
                {
                    LoadAssets();
                    break;
                }
                case GameState.Waiting:
                    try
                    {
                        foreach (var e in _gameManager.nextWorldEntities)
                        {
                            SpawnEntity(e);
                        }
                    }
                    finally
                    {
                        _gameManager.SetGameState(GameState.Running);
                    }

                    break;
            }
        }

        /// <summary>
        /// 在世界中生成Entity
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>如果返回null则生成失败</returns>
        public Entity SpawnEntity(AssetLocation id)
        {
            var entry = _gameManager.Registries.QueryRegistryEntry<EntityEntry>("entity", id);
            var e = entry?.SpawnEntity();
            if (e == null)
            {
                return null;
            }

            ActiveEntities.Add(e);
            if (e is IUpdatable up)
            {
                _updateEntities.Add(up);
            }

            return e;
        }

        private void LoadAssets()
        {
            var assetManager = _gameManager.Assets;
            assetManager.ReadyToLoad();
            assetManager.SetCompleteCallback(() => isAssetLoaded = true);

            var assetAddr = new List<string>();
            foreach (var entity in _gameManager.nextWorldEntities)
            {
                try
                {
                    assetAddr.AddRange(EntryDependInfos
                        .Registry
                        .SelectEntryDependInfo(new EntryLocation("entity", entity), "asset")
                        .Select(info => _gameManager.Registries.QueryRegistryEntry<AssetEntry>(info).ToString()));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            foreach (var addr in assetAddr)
            {
                assetManager.AddRequest(addr, null);
            }

            assetManager.StartLoad();
        }
    }
}