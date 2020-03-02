using System;
using System.Collections.Generic;
using System.Linq;
using Guirao.UltimateTextDamage;
using UnityEngine;

namespace Mercury
{
    [Serializable]
    public class TextDamageTypeWrapper
    {
        public string keyType;
        public int poolCount = 20;
        public string lable;
        public string name;
    }

    /// <summary>
    /// 世界数据，在每个场景都应该有一个
    /// </summary>
    public class WorldData : MonoBehaviour
    {
        /// <summary>
        /// 需要的资源是否加载
        /// </summary>
        public bool isAssetLoaded;

        [SerializeField] private UltimateTextDamageManager textDamageManager = null;
        [SerializeField] private List<TextDamageTypeWrapper> damageTexts = null;

        private GameManager _gameManager;

        /// <summary>
        /// 活动中的Entity
        /// </summary>
        public List<Entity> ActiveEntities { get; private set; }

        private void Awake()
        {
            ActiveEntities = new List<Entity>();
            _gameManager = GameManager.Instance;
            _gameManager.SetActiveWorld(this);
        }

        private void Update()
        {
            switch (_gameManager.nowState)
            {
                case GameState.Waiting when !isAssetLoaded:
                {
                    SetAssetManagerReady();
                    LoadEntityDependAssets();
                    LoadTextDamageAssets();
                    SetAssetManagerStartLoad();
                    break;
                }
                case GameState.Waiting:
                    try
                    {
                        foreach (var e in _gameManager.nextWorldEntities)
                        {
                            SpawnEntity(e);
                        }

                        SetupTextDamageManager();
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
            if (!e)
            {
                return null;
            }

            ActiveEntities.Add(e);
            return e;
        }

        private void SetAssetManagerReady()
        {
            var assetManager = _gameManager.Assets;
            assetManager.ReadyToLoad();
            assetManager.SetCompleteCallback(() => isAssetLoaded = true);
        }

        public void SetAssetManagerStartLoad() { _gameManager.Assets.StartLoad(); }

        /// <summary>
        /// 加载该世界实体需要的资源
        /// </summary>
        private void LoadEntityDependAssets()
        {
            var assetManager = _gameManager.Assets;
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
        }

        private void LoadTextDamageAssets()
        {
            if (!textDamageManager)
            {
                return;
            }

            foreach (var wrapper in damageTexts)
            {
                var registry = _gameManager.Registries;
                var assetEntry = registry.QueryRegistryEntry<AssetEntry>("asset", new AssetLocation(wrapper.lable, wrapper.name));
                if (assetEntry == null)
                {
                    throw new ArgumentException($"未知的伤害特效:{wrapper.lable}.effect.{wrapper.name}");
                }

                _gameManager.Assets.AddRequest(assetEntry.ToString(), null);
            }
        }

        private void SetupTextDamageManager()
        {
            if (!textDamageManager)
            {
                return;
            }

            textDamageManager.textTypes = damageTexts
                .Select(wrapper => new TextDamageType
                {
                    keyType = wrapper.keyType,
                    poolCount = wrapper.poolCount,
                    prefab = _gameManager
                        .Assets
                        .GetPrefab("effect", new AssetLocation(wrapper.lable, wrapper.name))
                        .GetComponent<UITextDamage>()
                })
                .ToList();
            textDamageManager.Init();
        }

        public static void ShowDamageText(string message, string type, Transform target)
        {
            //TODO:应该放在ui manager？
            GameManager.Instance.activeWorld.textDamageManager.Add(message, target, type);
        }
    }
}