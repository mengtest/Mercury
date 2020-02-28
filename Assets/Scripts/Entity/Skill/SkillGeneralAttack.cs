using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 平A
    /// </summary>
    public class SkillGeneralAttack : ISkill
    {
        /// <summary>
        /// 技能使用者
        /// </summary>
        private readonly ISkillOwner _skillUser;

        /// <summary>
        /// 技能使用者的攻击系统
        /// </summary>
        private readonly IAttackable _userAtkSys;

        /// <summary>
        /// 攻击范围
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
        /// 储存已经攻击过的敌人
        /// </summary>
        private readonly List<Collider2D> _attacked;

        /// <summary>
        /// 特效缓存池
        /// </summary>
        private readonly Queue<FollowTargetEffectCallback> _effectPool;

        /// <summary>
        /// 技能使用结束时间，和攻速有关
        /// </summary>
        private float _usingEndTime;

        /// <summary>
        /// Cd结束时间
        /// </summary>
        private float _cdEndTime;

        public AssetLocation Id { get; }
        public float PerUseTime { get; set; }
        public float PostUseTime { get; set; }
        public bool IsDone { get; private set; }

        /// <summary>
        /// 攻击范围偏移量
        /// </summary>
        public float AttackRangeOffset { get; set; }

        /// <summary>
        /// 攻速，值越大攻速越快
        /// </summary>
        public float AttackSpeed { get; set; } = 1;

        /// <summary>
        /// 伤害系数
        /// </summary>
        public float DamageCoe { get; set; }

        /// <summary>
        /// 伤害类型
        /// </summary>
        public DamageType DamageType { get; set; }

        public float Cd { get; set; }

        /// <summary>
        /// 可攻击的实体类型
        /// </summary>
        public EntityType AttackableType { get; } = EntityType.Enemy | EntityType.Neutral;

        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="id">临时id</param>
        /// <param name="skillUser">使用该技能的对象</param>
        /// <param name="attackable">使用该技能造成伤害的对象</param>
        /// <param name="userGo">使用者的GameObject</param>
        /// <param name="atkRange">攻击范围的预制体，注意，是预制体</param>
        /// <param name="effectPrefab">特效预制体，注意，是预制体</param>
        public SkillGeneralAttack(
            AssetLocation id,
            ISkillOwner skillUser,
            IAttackable attackable,
            GameObject userGo,
            GameObject atkRange,
            GameObject effectPrefab)
        {
            Id = id;
            _skillUser = skillUser;
            _userAtkSys = attackable;
            _userGo = userGo;
            _atkRng = Object.Instantiate(atkRange);
            _atkRng.SetActive(false);
            var cb = _atkRng.AddComponent<TriggerEventCallback>();
            cb.OnTriggerEnterEvent += OnAtkRngTriggerAttackable;
            cb.OnTriggerStayEvent += OnAtkRngTriggerAttackable;
            _effectPrefab = effectPrefab;
            _attacked = new List<Collider2D>(4);
            _effectPool = new Queue<FollowTargetEffectCallback>();
        }

        public bool CanUse()
        {
            //技能使用条件，当前没有使用中的技能 且 cd读条读完了
            return _skillUser.SkillSystem.UsingSkill == null && _cdEndTime <= Time.time;
        }

        public void OnPreUse()
        {
            IsDone = false; //重置是否使用完毕
            _usingEndTime = Time.time + 1f / AttackSpeed; //计算技能使用完毕的时间
            _atkRng.SetActive(true); //显示攻击范围
        }

        public void OnUsing()
        {
            var trans = _userGo.transform;
            var pos = trans.position;
            var scale = trans.localScale;
            var face = scale.x > 0 ? 1 : -1; //检查玩家面对的方向
            _atkRng.transform.position = new Vector3(pos.x + AttackRangeOffset * face, pos.y, pos.z);
            var rngScale = _atkRng.transform.localScale;
            _atkRng.transform.localScale = new Vector3(math.abs(rngScale.x) * face, rngScale.y, rngScale.z);
            if (_usingEndTime <= Time.time) //检查技能是否使用完毕
            {
                IsDone = true; //使用完毕
            }
        }

        public void OnPostUse()
        {
            _atkRng.SetActive(false); //隐藏攻击范围
            _attacked.Clear(); //清空已经被攻击的实体列表
            _cdEndTime = Time.time + Cd; //计算cd到期时间
        }

        /// <summary>
        /// 当攻击范围内有碰撞体时调用
        /// </summary>
        /// <param name="c">范围内的碰撞体</param>
        private void OnAtkRngTriggerAttackable(Collider2D c)
        {
            if (!c.CompareTag("entity")) //是实体
            {
                return;
            }

            var entity = c.GetComponent<Entity>();
            if (!AttackableType.HasFlag(entity.Type)) //是可以被攻击的实体
            {
                return;
            }

            if (!(entity is IAttackable atk)) //转换成可被攻击的实体
            {
                return;
            }

            if (_attacked.Contains(c)) //有没有被攻击过
            {
                return;
            }

            var dmg = _userAtkSys.DamageSystem.CalculateDamage(DamageCoe, DamageType); //计算伤害
            dmg = _userAtkSys.DamageSystem.Attack(dmg, atk); //攻击
            atk.DamageSystem.UnderAttack(dmg); //被攻击
            var effect = GetEffect();
            effect.target = c.transform; //设置特效跟随目标
            _attacked.Add(c); //添加到已被攻击的列表
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