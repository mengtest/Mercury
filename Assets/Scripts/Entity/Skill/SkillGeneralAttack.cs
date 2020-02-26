using UnityEngine;

namespace Mercury
{
    public class SkillGeneralAttack : ISkill
    {
        private readonly ISkillOwner _owner;
        private readonly GameObject _atkRng;
        private readonly GameObject _effectPrefab;
        private float _endTime;

        public AssetLocation Id { get; }
        public float PerUseTime { get; set; } = 2;
        public float PostUseTime { get; set; } = 2;
        public bool IsDone { get; private set; }

        public SkillGeneralAttack(
            AssetLocation id,
            ISkillOwner owner,
            GameObject atkRange,
            GameObject effectPrefab)
        {
            Id = id;
            _owner = owner;
            _atkRng = Object.Instantiate(atkRange);
            var cb = _atkRng.AddComponent<TriggerEventCallback>();
            cb.OnTriggerEnterEvent += OnAtkRngTriggerAttackable;
            cb.OnTriggerStayEvent += OnAtkRngTriggerAttackable;
            _effectPrefab = effectPrefab;
        }

        public bool CanUse() { return _owner.SkillSystem.UsingSkill == null; } //TODO:CD

        public void OnPreUse()
        {
            Debug.Log("使用");
            IsDone = false;
            _endTime = Time.time + 3;
        }

        public void OnUsing()
        {
            if (_endTime <= Time.time)
            {
                IsDone = true;
            }
        }

        public void OnPostUse() { Debug.Log("结束"); }

        private static void OnAtkRngTriggerAttackable(Collider2D c)
        {
            if (!c.CompareTag("entity"))
            {
                return;
            }

            Debug.Log(c.name);
        }
    }
}