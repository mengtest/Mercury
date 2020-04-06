using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 平A
    /// </summary>
    public class SkillGeneralAttack : MonoBehaviour, ISkill
    {
        /// <summary>
        /// 技能使用者
        /// </summary>
        private ISkillOwner _skillUser;

        /// <summary>
        /// 技能使用者的攻击系统
        /// </summary>
        private IAttackable _userAtkSys;

        /// <summary>
        /// 特效预制体
        /// </summary>
        [SerializeField] [Header("特效预制体.不可赋值,不可修改")]
        private GameObject _effectPrefab;

        /// <summary>
        /// 技能使用者的GameObject
        /// </summary>
        [SerializeField] [Header("技能使用者的GameObject.不可赋值，不可修改")]
        private GameObject _userGo;

        /// <summary>
        /// 动画组件
        /// </summary>
        private Animator _anim;

        /// <summary>
        /// 储存已经攻击过的敌人
        /// </summary>
        private List<Collider2D> _attacked;

        /// <summary>
        /// 特效缓存池
        /// </summary>
        private Queue<FollowTargetEffectCallback> _effectPool;

        /// <summary>
        /// Cd结束时间
        /// </summary>
        private float _cdEndTime;

        /// <summary>
        /// 前摇时间，单位：秒
        /// </summary>
        [Header("前摇时间，单位：秒")] public float perUseTime;

        /// <summary>
        /// 后摇时间，单位：秒
        /// </summary>
        [Header("后摇时间，单位：秒")] public float postUseTime;

        /// <summary>
        /// 技能是否施放完毕
        /// </summary>
        [Header("技能是否施放完毕.不可修改")] public bool isDone;

        /// <summary>
        /// 攻击范围偏移量
        /// </summary>
        [Header("攻击范围偏移量")] public Vector2 attackRangeOffset;

        /// <summary>
        /// 攻速，会修改动画速度
        /// </summary>
        [Header("攻速，会修改动画速度")] public float attackSpeed = 1;

        /// <summary>
        /// 伤害系数
        /// </summary>
        [Header("伤害系数")] public float damageCoe;

        /// <summary>
        /// 伤害类型
        /// </summary>
        [Header("伤害类型")] public DamageType damageType;

        /// <summary>
        /// 冷却时间
        /// </summary>
        [Header("冷却时间")] public float cd;

        /// <summary>
        /// 可攻击的实体类型
        /// </summary>
#if UNITY_EDITOR
        [MultiEnum]
#endif
        [Header("可攻击的实体类型")]
        public EntityType attackableType = EntityType.Enemy | EntityType.Neutral;

        public AssetLocation Id { get; private set; }
        bool ISkill.IsDone => isDone;
        float ISkill.PerUseTime => perUseTime;
        float ISkill.PostUseTime => postUseTime;

        private void OnTriggerEnter2D(Collider2D other) { OnAtkRngTriggerAttackable(other); }

        private void OnTriggerStay2D(Collider2D other) { OnAtkRngTriggerAttackable(other); }

        /// <summary>
        /// 实例化对象时初始化
        /// </summary>
        /// <param name="id">临时id</param>
        /// <param name="skillUser">使用该技能的对象</param>
        /// <param name="attackable">使用该技能造成伤害的对象</param>
        /// <param name="userGo">使用者的GameObject</param>
        /// <param name="effectPrefab">特效预制体，注意，是预制体</param>
        public SkillGeneralAttack Init(
            AssetLocation id,
            ISkillOwner skillUser,
            IAttackable attackable,
            GameObject userGo,
            GameObject effectPrefab)
        {
            Id = id;
            _skillUser = skillUser;
            _userAtkSys = attackable;
            _userGo = userGo;
            _effectPrefab = effectPrefab;
            _attacked = new List<Collider2D>(4);
            _effectPool = new Queue<FollowTargetEffectCallback>();
            _anim = GetComponent<Animator>();
            gameObject.SetActive(false);
            return this;
        }

        public bool CanUse()
        {
            if (_cdEndTime > Time.time)
            {
                return false;
            }

            if (_skillUser.SkillSystem.NowState == SkillState.Post)
            {
                return true;
            }

            return _skillUser.SkillSystem.NowState == SkillState.Normal;
        }

        public void OnPreUse()
        {
            isDone = false; //重置是否使用完毕
            gameObject.SetActive(true); //显示攻击范围
            _anim.speed = attackSpeed;
            OnUpdate();
        }

        public void OnUsing() { OnUpdate(); }

        private void OnUpdate()
        {
            var trans = _userGo.transform;
            var pos = trans.position; 
            var scale = trans.localScale;
            var face = scale.x > 0 ? 1 : -1; //检查玩家面对的方向
            var thisTrans = transform;
            thisTrans.position = new Vector3(pos.x + attackRangeOffset.x * face, pos.y + attackRangeOffset.y, pos.z);
            var rngScale = thisTrans.localScale;
            transform.localScale = new Vector3(math.abs(rngScale.x) * face, rngScale.y, rngScale.z);
        }

        public void OnPostUse()
        {
            gameObject.SetActive(false); //隐藏攻击范围
            _attacked.Clear(); //清空已经被攻击的实体列表
            _cdEndTime = Time.time + cd; //计算cd到期时间
            isDone = false;
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
            if (entity == _skillUser as Entity)
            {
                return;
            }

            if (!attackableType.HasFlag(entity.Type)) //是可以被攻击的实体
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

            var dmg = _userAtkSys.DamageSystem.CalculateDamage(damageCoe, damageType); //计算伤害
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
                var go = Instantiate(_effectPrefab); //新生成一个特效
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

        private void OnAnimPlayEnd()
        {
            gameObject.SetActive(false);
            isDone = true;
        }
    }
}