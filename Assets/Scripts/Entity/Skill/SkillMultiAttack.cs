using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    public class SkillMultiAttack : MonoBehaviour, ISkill
    {
        /// <summary>
        /// 技能使用者
        /// </summary>
        private ISkillOwner _skillUser;

        /// <summary>
        /// 技能使用者的攻击系统
        /// </summary>
        private IAttackable _attacker;

        /// <summary>
        /// 造成伤害特效的预制体
        /// </summary>
        private GameObject _effectPrefab;

        /// <summary>
        /// 技能使用者的GameObject
        /// </summary>
        private GameObject _userGo;

        /// <summary>
        /// 特效缓存池
        /// </summary>
        private Queue<FollowTargetEffectCallback> _effectPool; //TODO:封装成特效池

        private List<AtkInfo> _attacked;

        private float _cdEndTime; //TODO:封装成工具
        private Damage _dmg;
        private AssetLocation _id;
        public bool isDone;
        private Animator _anim;

        public float perUseTime;
        public float postUseTime;
        public float cd;
        public float attackSpeed = 1;
        public Vector2 attackRangeOffset;
        public EntityType attackableType = EntityType.Enemy | EntityType.Neutral;
        public int attackCount;
        public float attackInterval;
        public float damageCoe;
        public DamageType damageType;

        AssetLocation ISkill.Id => _id;
        bool ISkill.IsDone => isDone;
        float ISkill.PerUseTime => perUseTime;
        float ISkill.PostUseTime => postUseTime;

        private void OnTriggerEnter2D(Collider2D other) { OnAtkRngTriggerAttackable(other); }

        private void OnTriggerStay2D(Collider2D other) { OnAtkRngTriggerAttackable(other); }

        public SkillMultiAttack Init(
            AssetLocation id,
            ISkillOwner skillUser,
            IAttackable attackable,
            GameObject userGo,
            GameObject effectPrefab)
        {
            _id = id;
            _skillUser = skillUser;
            _attacker = attackable;
            _userGo = userGo;
            _effectPrefab = effectPrefab;
            _attacked = new List<AtkInfo>();
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
            gameObject.SetActive(true);
            isDone = false;
            _dmg = _attacker.DamageSystem.CalculateDamage(damageCoe, damageType);
            _anim.speed = attackSpeed;
            OnUpdate();
        }

        public void OnUsing() { OnUpdate(); }

        private void OnUpdate()
        {
            //TODO:封装成函数
            var trans = _userGo.transform;
            var pos = trans.position;
            var scale = trans.localScale;
            var face = scale.x > 0 ? 1 : -1;
            var thisTrans = transform;
            thisTrans.position = new Vector3(pos.x + attackRangeOffset.x * face, pos.y + attackRangeOffset.y, pos.z);
            var rngScale = thisTrans.localScale;
            transform.localScale = new Vector3(math.abs(rngScale.x) * face, rngScale.y, rngScale.z);
        }

        public void OnPostUse()
        {
            _cdEndTime = Time.time + cd;
            _attacked.Clear();
            gameObject.SetActive(false);
            isDone = false;
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

            if (!attackableType.HasFlag(entity.Type)) //是可以被攻击的实体
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
                info.nextAtkTime += attackInterval;
                _attacked[i] = info;
                return;
            }

            Attack(collider, atk);
            _attacked.Add(new AtkInfo(atk, attackCount - 1, Time.time + attackInterval));
        }

        private void Attack(Collider2D c, IAttackable target)
        {
            target.DamageSystem.UnderAttack(_attacker.DamageSystem.Attack(_dmg, target));
            var effect = GetEffect();
            effect.target = c.transform;
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

        private void OnAnimPlayEnd()
        {
            gameObject.SetActive(false);
            isDone = true;
        }
    }
}