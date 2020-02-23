using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mercury
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameState
    {
        Init,
        Loading,
        Running,
        Pause
    }

    /// <summary>
    /// 管理整个游戏的所有行为和数据
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState nowState = GameState.Init;

        public WorldData activeWorld;

        public PlayerInterface raceter; //TODO:临时存放，需要asset manager

        public EventSystem EventBus { get; private set; }
        public RegisterManager Registries { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Addressables.LoadAssetAsync<GameObject>("entity.raceter").Completed += q => raceter = q.Result.AddComponent<PlayerInterface>();
            EventBus = new EventSystem();
            Registries = new RegisterManager(this);
            EventBus.Init();
        }

        private void Start()
        {
            //初始化注册表
            Registries.AddRegistry(new RegistryImpl<EntityEntry>("entity"));
            Registries.Init();
        }

        private void Update()
        {
            if (raceter)
            {
                try
                {
                    raceter = Instantiate(raceter);
                    EntityEntry.Init();
                    var e = activeWorld.SpawnEntity(new AssetLocation("mercury", "raceter"));
                }
                finally
                {
                    raceter = null;
                }
            }
        }

        public void SetActiveWorld(WorldData world) { activeWorld = world; }

        /// <summary>
        /// 检查游戏状态
        /// </summary>
        /// <param name="state">需要的状态</param>
        /// <param name="message">状态不匹配时抛出异常的信息</param>
        /// <exception cref="InvalidOperationException">状态不匹配</exception>
        public void CheckState(GameState state, string message)
        {
            if (nowState != state)
            {
                throw new InvalidOperationException($"当前状态:{nowState}，信息:" + message);
            }
        }
    }
}