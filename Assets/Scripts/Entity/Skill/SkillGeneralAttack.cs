using System;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mercury
{
    public class SkillGeneralAttack : ISkill
    {
        private readonly ISkillOwner _owner;
        private readonly IAttackable _ownerAtk;
        private readonly GameObject _atkRng;
        private readonly GameObject _effectPrefab;
        private readonly GameObject _ownerGo;
        private float _endTime;

        public AssetLocation Id { get; }
        public float PerUseTime { get; set; }
        public float PostUseTime { get; set; }
        public bool IsDone { get; private set; }
        public float PosOffset { get; set; }
        public float AttackSpeed { get; set; } = 1;
        public EntityType AttackableType { get; } = EntityType.Enemy | EntityType.Neutral;

        public SkillGeneralAttack(
            AssetLocation id,
            ISkillOwner owner,
            GameObject ownerGo,
            GameObject atkRange,
            GameObject effectPrefab)
        {
            Id = id;
            _owner = owner;
            if (!(_owner is IAttackable atk))
            {
                throw new InvalidOperationException("来源必须可攻击");
            }

            _ownerAtk = atk;
            _ownerGo = ownerGo;
            _atkRng = Object.Instantiate(atkRange);
            _atkRng.SetActive(false);
            var cb = _atkRng.AddComponent<TriggerEventCallback>();
            cb.OnTriggerEnterEvent += OnAtkRngTriggerAttackable;
            cb.OnTriggerStayEvent += OnAtkRngTriggerAttackable;
            _effectPrefab = effectPrefab;
        }

        public bool CanUse() { return _owner.SkillSystem.UsingSkill == null; } //TODO:CD

        public void OnPreUse()
        {
            IsDone = false;
            _endTime = Time.time + 1f / AttackSpeed;
            _atkRng.SetActive(true);
        }

        public void OnUsing()
        {
            var trans = _ownerGo.transform;
            var pos = trans.position;
            var scale = trans.localScale;
            var face = scale.x > 0 ? 1 : -1;
            _atkRng.transform.position = new Vector3(pos.x + PosOffset * face, pos.y, pos.z);
            var rngScale = _atkRng.transform.localScale;
            _atkRng.transform.localScale = new Vector3(math.abs(rngScale.x) * face, rngScale.y, rngScale.z);
            if (_endTime <= Time.time)
            {
                IsDone = true;
            }
        }

        public void OnPostUse() { _atkRng.SetActive(false); }

        private void OnAtkRngTriggerAttackable(Collider2D c)
        {
            if (!c.CompareTag("entity"))
            {
                return;
            }

            var entity = c.GetComponent<EntityReference>().Entity;
            if (!AttackableType.HasFlag(entity.Type))
            {
                return;
            }

            if (!(entity is IAttackable atk))
            {
                return;
            }

            var dmg = _ownerAtk.DamageSystem.DealDamage(new Damage(_ownerAtk, 1, 0, DamageType.True), atk);
            atk.DamageSystem.UnderAttack(dmg);
        }
    }
}