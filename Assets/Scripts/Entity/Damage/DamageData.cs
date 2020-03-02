using System;
using UnityEngine;

namespace Mercury
{
    [Serializable]
    public class DamageData : MonoBehaviour, IEntityComponent
    {
        /// <summary>
        /// 最大血量
        /// </summary>
        public float maxHealth;

        /// <summary>
        /// 每秒血量变化
        /// </summary>
        public float healthRecover;

        /// <summary>
        /// 物理攻击力
        /// </summary>
        public float physicsAttack;

        /// <summary>
        /// 魔法攻击力
        /// </summary>
        public float magicAttack;

        /// <summary>
        /// 暴击伤害系数
        /// </summary>
        public float critCoe;

        /// <summary>
        /// 暴击率系数
        /// </summary>
        public int critPrCoe;
    }
}