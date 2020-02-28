using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 剑意系统
    /// </summary>
    public class SwordWillSystem : IEntitySystem, IUpdatable
    {
        /// <summary>
        /// 伤害计算器
        /// </summary>
        private readonly IDamageCompute _compute;

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
        private int _will;

        /// <summary>
        /// 是否正在收剑
        /// </summary>
        private bool _isRecycling;

        /// <summary>
        /// 完成收剑的时间
        /// </summary>
        private float _completeRecycleTime;

        /// <summary>
        /// true是拔刀，false是收刀
        /// </summary>
        public bool SwordState { get; private set; }

        /// <summary>
        /// 拔刀时的暴击率收益，单位：暴击率/剑意
        /// </summary>
        public float CritPrPerWill { get; set; } = 0.001f;

        /// <summary>
        /// 收刀时伤害收益，单位：收益/剑意
        /// </summary>
        public float DamageIncomePerWill { get; set; } = 0.005f;

        /// <summary>
        /// 收刀时，恢复剑意的时间间隔，单位：秒
        /// </summary>
        public float RecycleStateIncreaseWillInterval { get; set; } = 1;

        /// <summary>
        /// 拔刀时，n秒内未再次攻击敌人，会损失剑意的时间，单位：秒
        /// </summary>
        public float AfterAttackLoseWillTime { get; set; } = 1;

        /// <summary>
        /// 拔刀状态，n秒内未再次攻击敌人，会损失剑意的时间间隔，单位：秒
        /// </summary>
        public float DrawStateLoseWillInterval { get; set; } = 0.5f;

        /// <summary>
        /// 最大剑意
        /// </summary>
        public int MaxWill { get; set; } = 100;

        /// <summary>
        /// 收刀所需时间
        /// </summary>
        public float RecycleSwordTime { get; set; } = 1;

        /// <summary>
        /// 拔刀时，每次攻击增加的剑意
        /// </summary>
        public int WillIncreasePerAttack { get; set; } = 2;

        /// <summary>
        /// 剑意
        /// </summary>
        public int Will
        {
            get => _will;
            private set
            {
                var tryAdd = _will + value;
                _will = math.max(0, math.min(MaxWill, tryAdd));
            }
        }

        public SwordWillSystem(IDamageSystem system, IDamageCompute compute)
        {
            _compute = compute;
            system.OnDealDamage += OnAttack;
            _shouldGetWillTime = Time.time + RecycleStateIncreaseWillInterval;
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
            if (SwordState) //拔刀
            {
                Will = WillIncreasePerAttack; //增加剑意
                RefreshCritPr(Will * CritPrPerWill); //刷新暴击率
                _shouldReduceWillTime = now + AfterAttackLoseWillTime + DrawStateLoseWillInterval; //刷新剑意减少开始时间
            }
            else
            {
                if (Will >= MaxWill) //到达最大剑意
                {
                    var dmg = attack.damage;
                    dmg.ExtraCritValue = _compute.GetCritDamage(dmg.Value); //计算暴击，直接赋值
                    attack.Result = dmg; //设置最终伤害
                }

                DrawSword(); //拔刀
            }
        }

        public void OnUpdate()
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

            if (SwordState) //拔刀
            {
                //如果没到剑意开始减少的时间 && 没到减少剑意的时刻
                if (!(_lastAtkTime + AfterAttackLoseWillTime < now) || !(_shouldReduceWillTime < now))
                {
                    return;
                }

                _shouldReduceWillTime += DrawStateLoseWillInterval; //刷新减少剑意的时刻
                Will = -5;
                RefreshCritPr(Will * CritPrPerWill); //刷新暴击率
            }
            else
            {
                if (!(_shouldGetWillTime <= now)) //没到恢复剑意的时间
                {
                    return;
                }

                Will = 20;
                _shouldGetWillTime += RecycleStateIncreaseWillInterval; //刷新下次恢复剑意的时间
                RefreshDmg(Will * DamageIncomePerWill); //刷新伤害
            }
        }

        /// <summary>
        /// 拔刀
        /// </summary>
        public void DrawSword()
        {
            RefreshDmg(0);
            _shouldGetWillTime = 0;
            SwordState = true;
            _will /= 2;
            RefreshCritPr(Will * CritPrPerWill);
            _shouldReduceWillTime = Time.time;
        }

        /// <summary>
        /// 收刀
        /// </summary>
        public void RecycleSword()
        {
            RefreshCritPr(0);
            _will = 0;
            RefreshDmg(Will * DamageIncomePerWill);
            _lastAtkTime = 0;
            _shouldReduceWillTime = 0;
            _shouldGetWillTime = Time.time + RecycleStateIncreaseWillInterval;
            SwordState = false;
        }

        /// <summary>
        /// 开始收刀
        /// </summary>
        public void StartRecycleSword()
        {
            if (!SwordState)
            {
                return;
            }

            _isRecycling = true;
            _completeRecycleTime = Time.time + RecycleSwordTime;
        }

        private void RefreshCritPr(float newCritPr)
        {
            if (math.abs(_lastAddCritPrInc) > 0.0000001f)
            {
                _compute.RemoveCritPr(_lastAddCritPrInc, CritPrType.Percentage);
            }

            _compute.AddCritPr(newCritPr, CritPrType.Percentage);
            _lastAddCritPrInc = newCritPr;
        }

        private void RefreshDmg(float newDmg)
        {
            if (math.abs(_lastAddCritPrInc) > 0.0000001f)
            {
                _compute.RemoveDamageIncome(_lastAddDmg, DamageType.Physics);
            }

            _compute.AddDamageIncome(newDmg, DamageType.Physics);
            _lastAddDmg = newDmg;
        }
    }
}