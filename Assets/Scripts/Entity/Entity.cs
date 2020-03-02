using System;
using UnityEngine;

namespace Mercury
{
    [Flags]
    public enum EntityType
    {
        Player = 00,
        Enemy = 01,
        Neutral = 10
    }

    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        public AssetLocation Id { get; set; }

        public EntityType Type { get; set; }

        protected void Start()
        {
            if (Id == null)
            {
                throw new ArgumentException($"实体{name}必须设置一个id");
            }
        }
    }
}