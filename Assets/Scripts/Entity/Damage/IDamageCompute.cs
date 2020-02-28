using System;

namespace Mercury
{
    /// <summary>
    /// 伤害计算器
    /// </summary>
    public interface IDamageCompute
    {
        /// <summary>
        /// 最大生命值
        /// </summary>
        DataChange<float> MaxHealth { get; }

        void AddMaxHealth(float value);

        bool RemoveMaxHealth(float value);

        /// <summary>
        /// 每秒生命回复
        /// </summary>
        DataChange<float> HealthRecover { get; }

        void AddHealthRecover(float value);

        bool RemoveHealthRecover(float value);

        /// <summary>
        /// 暴击伤害系数
        /// </summary>
        DataChange<float> CritCoe { get; }

        void AddCritCoe(float value);

        bool RemoveCritCoe(float value);

        /// <summary>
        /// 暴击率
        /// </summary>
        DataChange<float> CritPr { get; }

        void AddCritPr(float value, CritPrType type);

        bool RemoveCritPr(float value, CritPrType type);

        /// <summary>
        /// 攻击力
        /// </summary>
        /// <param name="type">类型</param>
        DataChange<float> GetAttack(DamageType type);

        void AddAttack(float value, DamageType type);

        bool RemoveAttack(float value, DamageType type);

        /// <summary>
        /// 伤害增益
        /// </summary>
        /// <param name="type">伤害类型</param>
        float GetDamageIncome(DamageType type);

        void AddDamageIncome(float value, DamageType type);

        bool RemoveDamageIncome(float value, DamageType type);

        /// <summary>
        /// 获取原始伤害
        /// </summary>
        /// <param name="coe">伤害系数</param>
        /// <param name="type">伤害类型</param>
        float GetDamage(float coe, DamageType type);

        /// <summary>
        /// 获取增益后额外伤害 额外伤害 额外伤害。
        /// 强调三遍，是额外伤害
        /// </summary>
        /// <param name="rawDamage">伤害系数</param>
        /// <param name="type">伤害类型</param>
        float GetExtraDamageIncome(float rawDamage, DamageType type);

        /// <summary>
        /// 获取暴击的额外伤害 额外伤害 额外伤害。
        /// 强调三遍，是额外伤害
        /// </summary>
        /// <param name="damage">未暴击时伤害</param>
        float GetExtraCritDamage(float damage);
    }

    public class DamageComputeImpl : IDamageCompute
    {
        private readonly DamageData _damage;
        private readonly CacheData<float>[] _data;

        public DamageComputeImpl(DamageData damageData)
        {
            _damage = damageData;
            _data = new CacheData<float>[10];
            _data[0] = new ChainAdd(); //phy atk
            _data[1] = new ChainAdd(); //mag atk
            _data[2] = new ChainMulti(); //phy dmg
            _data[3] = new ChainMulti(); //mag dmg
            _data[4] = new ChainMulti(); //all dmg
            _data[5] = new ChainMulti(); //crit coe
            _data[6] = new ChainAdd(); //crit pr val
            _data[7] = new ChainAdd(); //crit pr per
            _data[8] = new ChainAdd(); //mx hp
            _data[9] = new ChainAdd(); //hp rec
        }

        public DataChange<float> MaxHealth => new DataChange<float>(_damage.maxHealth, _data[8].Cache);
        public void AddMaxHealth(float value) => _data[8].AddData(value);
        public bool RemoveMaxHealth(float value) => _data[8].RemoveData(value);
        public DataChange<float> HealthRecover => new DataChange<float>(_damage.healthRecover, _data[9].Cache);
        public void AddHealthRecover(float value) => _data[9].AddData(value);
        public bool RemoveHealthRecover(float value) => _data[9].RemoveData(value);
        public DataChange<float> CritCoe => new DataChange<float>(_damage.critCoe, _data[5].Cache + 1f);
        public void AddCritCoe(float value) => _data[5].AddData(value);
        public bool RemoveCritCoe(float value) => _data[5].RemoveData(value);
        public DataChange<float> CritPr => new DataChange<float>(Misc.RecoverCritPr(_damage.critPrCoe), Misc.RecoverCritPr(_data[6].Cache) + _data[7].Cache);
        public void AddCritPr(float value, CritPrType type) => _data[(int) type + 6].AddData(value);
        public bool RemoveCritPr(float value, CritPrType type) { return _data[(int) type + 6].RemoveData(value); }

        public DataChange<float> GetAttack(DamageType type)
        {
            switch (type)
            {
                case DamageType.Physics:
                    var phyAtk = _damage.physicsAttack;
                    return new DataChange<float>(phyAtk, _data[0].Cache * phyAtk);
                case DamageType.Magic:
                    var magAtk = _damage.magicAttack;
                    return new DataChange<float>(magAtk, _data[1].Cache * magAtk);
                case DamageType.True:
                    throw new InvalidOperationException("不存在真伤攻击力");
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void AddAttack(float value, DamageType type)
        {
            if (type == DamageType.True)
            {
                throw new InvalidOperationException("不存在真伤攻击力");
            }

            _data[(int) type].AddData(value);
        }

        public bool RemoveAttack(float value, DamageType type)
        {
            if (type == DamageType.True)
            {
                throw new InvalidOperationException("不存在真伤攻击力");
            }

            return _data[(int) type].RemoveData(value);
        }

        public float GetDamageIncome(DamageType type) => _data[(int) type + 2].Cache + 1;
        public void AddDamageIncome(float value, DamageType type) => _data[(int) type + 2].AddData(value);
        public bool RemoveDamageIncome(float value, DamageType type) => _data[(int) type + 2].RemoveData(value);

        public float GetDamage(float coe, DamageType type)
        {
            var atk = Misc.AddDataChange(GetAttack(type));
            return coe * 0.01f * atk;
        }

        public float GetExtraDamageIncome(float rawDamage, DamageType type)
        {
            var result = rawDamage * GetDamageIncome(type);
            if (type != DamageType.True)
            {
                result *= GetDamageIncome(DamageType.True);
            }

            return result - rawDamage;
        }

        public float GetExtraCritDamage(float damage) => damage * Misc.MultiDataChange(CritCoe) - damage;
    }
}