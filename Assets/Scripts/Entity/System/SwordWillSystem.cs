using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 剑意系统
    /// </summary>
    public class SwordWillSystem : MonoBehaviour, IEntitySystem
    {
        /// <summary>
        /// 伤害计算器
        /// </summary>
        private IDamageCompute _compute;

        /// <summary>
        /// 最后一次添加的暴击率
        /// </summary>
        private float _lastAddCritPrInc;

        /// <summary>
        /// 最后添加伤害收益的时间
        /// </summary>
        private float _lastAtkTime;

        /// <summary>
        /// 在拔刀状态下，超过n秒没有攻击敌人，应该减少剑意的时刻
        /// </summary>
        private float _shouldReduceWillTime;

        /// <summary>
        /// 在收刀状态下，应该增加剑意的时刻
        /// </summary>
        private float _shouldGetWillTime;

        /// <summary>
        /// 最后添加的伤害收益
        /// </summary>
        private float _lastAddDmg;

        /// <summary>
        /// 剑意
        /// </summary>
        [SerializeField] [Header("剑意")] private int _will;

        /// <summary>
        /// 是否正在收剑
        /// </summary>
        private bool _isRecycling;

        /// <summary>
        /// 完成收剑的时间
        /// </summary>
        private float _completeRecycleTime;

        /// <summary>
        /// 最大剑意
        /// </summary>
        [Header("最大剑意")] public int maxWill = 100;

        /// <summary>
        /// true是拔刀，false是收刀
        /// </summary>
        [Header("true是拔刀，false是收刀")] public bool swordState;

        /// <summary>
        /// 拔刀时的暴击率收益，单位：暴击率/剑意
        /// </summary>
        [Header("拔刀时的暴击率收益，单位：暴击率/剑意")] public float critPrPerWill = 0.001f;

        /// <summary>
        /// 拔刀时，n秒内未再次攻击敌人，会损失剑意的时间，单位：秒
        /// </summary>
        [Header("拔刀时，n秒内未再次攻击敌人，会损失剑意的时间，单位：秒")]
        public float afterAttackLoseWillTime = 1;

        /// <summary>
        /// 拔刀状态，n秒内未再次攻击敌人，会损失剑意的时间间隔，单位：秒
        /// </summary>
        [Header("拔刀状态，n秒内未再次攻击敌人，会损失剑意的时间间隔，单位：秒")]
        public float drawStateLoseWillInterval = 0.5f;
        
        /// <summary>
        /// 拔刀时，每次攻击增加的剑意
        /// </summary>
        [Header("拔刀时，每次攻击增加的剑意")] public int willIncreasePerAttack = 2;
        
        /// <summary>
        /// 收刀所需时间
        /// </summary>
        [Header("收刀所需时间")] public float recycleSwordTime = 1;
        
        /// <summary>
        /// 收刀时伤害收益，单位：收益/剑意
        /// </summary>
        [Header("收刀时伤害收益，单位：收益/剑意")] public float damageIncomePerWill = 0.005f;

        /// <summary>
        /// 收刀时，恢复剑意的时间间隔，单位：秒
        /// </summary>
        [Header("收刀时，恢复剑意的时间间隔，单位：秒")] public float recycleStateIncreaseWillInterval = 1;

        /// <summary>
        /// 剑意
        /// </summary>
        public int Will
        {
            get => _will;
            private set
            {
                var tryAdd = _will + value;
                _will = math.max(0, math.min(maxWill, tryAdd));
            }
        }

        public SwordWillSystem Init(IDamageSystem system, IDamageCompute compute)
        {
            _compute = compute;
            system.OnAttack += OnAttack;
            _shouldGetWillTime = Time.time + recycleStateIncreaseWillInterval;
            return this;
        }

        /// <summary>
        /// 攻击敌人时触发
        /// </summary>
        private void OnAttack(object sender, EntityAttackEvent.Attack attack)
        {
            if (_isRecycling) //处于回收状态时什么都不做
            {
                return;
            }

            var now = Time.time;
            _lastAtkTime = now; //刷新上次攻击时间
            if (swordState) //拔刀
            {
                Will = willIncreasePerAttack; //增加剑意
                RefreshCritPr(Will * critPrPerWill); //刷新暴击率
                _shouldReduceWillTime = now + afterAttackLoseWillTime + drawStateLoseWillInterval; //刷新剑意减少开始时间
                return;
            }

            var dmg = attack.damage;
            if (Will >= maxWill) //到达最大剑意
            {
                dmg.ExtraCritValue = _compute.GetExtraCritDamage(dmg.Value); //计算暴击，直接赋值
            }

            DrawSword(); //拔刀
            attack.Result = dmg; //修改伤害
        }

        private void Update()
        {
            //Debug.Log($"will:{Will} state:{SwordState} crit:{_lastAddCritPrInc} dmg:{_lastAddDmg}");
            var now = Time.time;
            if (_isRecycling) //正在收刀
            {
                if (!(_completeRecycleTime <= now)) //没到收刀完成时间
                {
                    return; //什么都不用做
                }

                RecycleSword(); //收刀
                _isRecycling = false; //退出收刀状态
            }

            if (swordState) //拔刀
            {
                //如果没到剑意开始减少的时间 && 没到减少剑意的时刻
                if (!(_lastAtkTime + afterAttackLoseWillTime < now) || !(_shouldReduceWillTime < now))
                {
                    return;
                }

                _shouldReduceWillTime += drawStateLoseWillInterval; //刷新减少剑意的时刻
                Will = -5;
                RefreshCritPr(Will * critPrPerWill); //刷新暴击率
            }
            else
            {
                if (!(_shouldGetWillTime <= now)) //没到恢复剑意的时间
                {
                    return;
                }

                Will = 20;
                _shouldGetWillTime += recycleStateIncreaseWillInterval; //刷新下次恢复剑意的时间
                RefreshDmg(Will * damageIncomePerWill); //刷新伤害
            }
        }

        /// <summary>
        /// 拔刀
        /// </summary>
        public void DrawSword()
        {
            RefreshDmg(0);
            _shouldGetWillTime = 0;
            swordState = true;
            _will /= 2;
            RefreshCritPr(Will * critPrPerWill);
            _shouldReduceWillTime = Time.time;
        }

        /// <summary>
        /// 收刀
        /// </summary>
        public void RecycleSword()
        {
            RefreshCritPr(0);
            _will = 0;
            RefreshDmg(Will * damageIncomePerWill);
            _lastAtkTime = 0;
            _shouldReduceWillTime = 0;
            _shouldGetWillTime = Time.time + recycleStateIncreaseWillInterval;
            swordState = false;
        }

        /// <summary>
        /// 开始收刀
        /// </summary>
        public void StartRecycleSword()
        {
            if (!swordState)
            {
                return;
            }

            _isRecycling = true;
            _completeRecycleTime = Time.time + recycleSwordTime;
        }

        private void RefreshCritPr(float newCritPr)
        {
            if (math.abs(_lastAddCritPrInc - newCritPr) < 0.0000001f)
            {
                return;
            }

            if (math.abs(_lastAddCritPrInc) > 0.0000001f)
            {
                _compute.RemoveCritPr(_lastAddCritPrInc, CritPrType.Percentage);
            }

            if (math.abs(newCritPr) > 0.0000001f)
            {
                _compute.AddCritPr(newCritPr, CritPrType.Percentage);
            }

            _lastAddCritPrInc = newCritPr;
        }

        private void RefreshDmg(float newDmg)
        {
            if (math.abs(_lastAddDmg - newDmg) < 0.0000001f)
            {
                return;
            }

            if (math.abs(_lastAddDmg) > 0.0000001f)
            {
                _compute.RemoveDamageIncome(_lastAddDmg, DamageType.Physics);
            }

            if (math.abs(newDmg) > 0.0000001f)
            {
                _compute.AddDamageIncome(newDmg, DamageType.Physics);
            }

            _lastAddDmg = newDmg;
        }
    }
}