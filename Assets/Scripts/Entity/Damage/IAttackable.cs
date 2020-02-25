namespace Mercury
{
    public interface IAttackable //TODO:死亡？
    {
        /// <summary>
        /// 当前血量
        /// </summary>
        float Health { get; set; }

        /// <summary>
        /// 伤害原始数据
        /// </summary>
        DamageData DamageRawData { get; }

        /// <summary>
        /// 伤害计算
        /// </summary>
        IDamageCompute DamageCompute { get; }

        /// <summary>
        /// 伤害系统
        /// </summary>
        IDamageSystem DamageSystem { get; }
    }
}