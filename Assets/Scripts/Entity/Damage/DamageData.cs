using System;
using UnityEngine;

namespace Mercury
{
    [Serializable]
    public class DamageData : MonoBehaviour, IEntityData
    {
        /// <summary>
        /// 最大血量
        /// </summary>
        [Header("最大血量")] public float maxHealth;

        /// <summary>
        /// 每秒血量变化
        /// </summary>
        [Header("每秒血量变化")] public float healthRecover;

        /// <summary>
        /// 物理攻击力
        /// </summary>
        [Header("物理攻击力")] public float physicsAttack;

        /// <summary>
        /// 魔法攻击力
        /// </summary>
        [Header("魔法攻击力")] public float magicAttack;

        /// <summary>
        /// 暴击伤害系数
        /// </summary>
        [Header("暴击伤害系数")] public float critCoe;

        /// <summary>
        /// 暴击率系数
        /// </summary>
        [Header("暴击率系数")] public int critPrCoe;
    }
}