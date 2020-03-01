using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    public class SkillMultiAttack : ISkill
    {
        /// <summary>
        /// 技能使用者
        /// </summary>
        private readonly ISkillOwner _skillUser;

        /// <summary>
        /// 技能使用者的攻击系统
        /// </summary>
        private readonly IAttackable _attacker;

        /// <summary>
        /// 攻击范围，是实例
        /// </summary>
        private readonly GameObject _atkRng;

        /// <summary>
        /// 造成伤害特效的预制体
        /// </summary>
        private readonly GameObject _effectPrefab;

        /// <summary>
        /// 技能使用者的GameObject
        /// </summary>
        private readonly GameObject _userGo;

        /// <summary>
        /// 特效缓存池
        /// </summary>
        private readonly Queue<FollowTargetEffectCallback> _effectPool; //TODO:封装成特效池

        private readonly List<AtkInfo> _attacked;

        private float _cdEndTime; //TODO:封装成工具
        private float _usingEndTime;
        private Damage _dmg;
        private float _atkIntervalTime;

        public AssetLocation Id { get; }
        public float PerUseTime { get; set; }
        public float PostUseTime { get; set; }
        public bool IsDone { get; private set; }
        public float Cd { get; set; }
        public float AttackSpeed { get; set; } = 1;
        public float AttackRangeOffset { get; set; }
        public EntityType AttackableType { get; set; } = EntityType.Enemy | EntityType.Neutral;
        public int AttackCount { get; set; }
        public float AttackInterval { get; set; }
        public float DamageCoe { get; set; }
        public DamageType DamageType { get; set; }

        public SkillMultiAttack(
            AssetLocation id,
            ISkillOwner skillUser,
            IAttackable attackable,
            GameObject userGo,
            GameObject atkRange,
            GameObject effectPrefab)
        {
            Id = id;
            _skillUser = skillUser;
            _attacker = attackable;
            _userGo = userGo;
            _atkRng = atkRange;
            var effcb = atkRange.GetComponent<EffectCallback>();
            if (effcb)
            {
                effcb.EventA += () => _atkRng.SetActive(false);
            }

            _atkRng.SetActive(false);
            var cb = _atkRng.AddComponent<TriggerEventCallback>();
            cb.OnTriggerEnterEvent += OnAtkRngTriggerAttackable;
            cb.OnTriggerStayEvent += OnAtkRngTriggerAttackable;
            _effectPrefab = effectPrefab;
            _attacked = new List<AtkInfo>();
            _effectPool = new Queue<FollowTargetEffectCallback>();
        }

        public bool CanUse() { return _skillUser.SkillSystem.UsingSkill == null && _cdEndTime <= Time.time; }

        public void OnPreUse()
        {
            var atkTime = 1f / AttackSpeed;
            _usingEndTime = Time.time + 1f / AttackSpeed;
            _atkRng.SetActive(true);
            IsDone = false;
            _dmg = _attacker.DamageSystem.CalculateDamage(DamageCoe, DamageType);
            if (AttackCount * AttackInterval > atkTime)
            {
                Debug.LogWarning($"造成所有伤害所需时间{AttackCount * AttackInterval}超过了技能施放时间{atkTime}");
                _atkIntervalTime = atkTime / AttackCount;
            }
            else
            {
                _atkIntervalTime = AttackInterval;
            }
        }

        public void OnUsing()
        {
            //TODO:封装成函数
            var trans = _userGo.transform;
            var pos = trans.position;
            var scale = trans.localScale;
            var face = scale.x > 0 ? 1 : -1;
            _atkRng.transform.position = new Vector3(pos.x + AttackRangeOffset * face, pos.y, pos.z);
            var rngScale = _atkRng.transform.localScale;
            _atkRng.transform.localScale = new Vector3(math.abs(rngScale.x) * face, rngScale.y, rngScale.z);
            if (_usingEndTime <= Time.time)
            {
                IsDone = true;
            }
        }

        public void OnPostUse()
        {
            _cdEndTime = Time.time + Cd;
            _attacked.Clear();
            _atkRng.SetActive(false);
            IsDone = false;
        }

        private void OnAtkRngTriggerAttackable(Collider2D collider)
        {
            if (!collider.CompareTag("entity")) //是实体
            {
                return;
            }

            var entity = collider.GetComponent<Entity>();
            if (entity == _skillUser as Entity)
            {
                return;
            }

            if (!AttackableType.HasFlag(entity.Type)) //是可以被攻击的实体
            {
                return;
            }

            if (!(entity is IAttackable atk)) //转换成可被攻击的实体
            {
                return;
            }

            for (var i = 0; i < _attacked.Count; i++)
            {
                var info = _attacked[i];
                if (info.attackedEntity != atk)
                {
                    continue;
                }

                if (info.count <= 0)
                {
                    return;
                }

                if (info.nextAtkTime > Time.time)
                {
                    return;
                }

                Attack(collider, atk);
                info.count -= 1;
                info.nextAtkTime += _atkIntervalTime;
                _attacked[i] = info;
                return;
            }

            Attack(collider, atk);
            _attacked.Add(new AtkInfo(atk, AttackCount - 1, Time.time + _atkIntervalTime));
        }

        private void Attack(Collider2D collider, IAttackable target)
        {
            target.DamageSystem.UnderAttack(_attacker.DamageSystem.Attack(_dmg, target));
            var effect = GetEffect();
            effect.target = collider.transform;
        }

        private struct AtkInfo
        {
            internal readonly IAttackable attackedEntity;
            internal int count;
            internal float nextAtkTime;

            public AtkInfo(IAttackable attackedEntity, int count, float nextAtkTime)
            {
                this.attackedEntity = attackedEntity;
                this.count = count;
                this.nextAtkTime = nextAtkTime;
            }
        }

        /// <summary>
        /// 获取一个攻击特效
        /// </summary>
        private FollowTargetEffectCallback GetEffect()
        {
            FollowTargetEffectCallback result;
            if (_effectPool.Count != 0) //特效池中还有剩余
            {
                result = _effectPool.Dequeue();
            }
            else
            {
                var go = Object.Instantiate(_effectPrefab); //新生成一个特效
                result = go.GetComponent<FollowTargetEffectCallback>(); //获取特效回调
                result.EventA += () => //当特效结束播放时触发
                {
                    _effectPool.Enqueue(result); //放回池中
                    result.gameObject.SetActive(false); //隐藏
                };
            }

            result.gameObject.SetActive(true); //显示特效
            return result;
        }
    }
}