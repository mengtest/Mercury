using System;
using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameState
    {
        Init,
        ReadyToLoad,
        Loading,
        Running,
        Waiting,
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
        public List<AssetLocation> nextWorldEntities;

        public EventSystem EventBus { get; private set; }
        public RegisterManager Registries { get; private set; }
        public AssetManager Assets { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            EventBus = new EventSystem();
            Registries = new RegisterManager(this);
            EventBus.Init();
            Assets = new AssetManager(this);
        }

        private void Start()
        {
            Registries.Init();
            nextWorldEntities = new List<AssetLocation>
            {
                new AssetLocation("mercury", "raceter")
            };
            SetGameState(GameState.Waiting);
        }

        public void SetActiveWorld(WorldData newWorld)
        {
            //TODO:触发事件
            activeWorld = newWorld;
        }

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

        public void SetGameState(GameState newState)
        {
            //TODO:触发事件
            nowState = newState;
        }
    }
}