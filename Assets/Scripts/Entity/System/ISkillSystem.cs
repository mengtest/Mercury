using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 技能系统
    /// </summary>
    public interface ISkillSystem : IEntitySystem, IUpdatable
    {
        event EventHandler<EntitySkillEvent.PreUse> OnPreUseSkill;

        event EventHandler<EntitySkillEvent.Using> OnUsingSkill;

        event EventHandler<EntitySkillEvent.PostUse> OnPostUseSkill;

        /// <summary>
        /// 正在使用的技能，可能为null。如果是null则没有使用中技能
        /// </summary>
        ISkill UsingSkill { get; }

        /// <summary>
        /// 添加一个技能
        /// </summary>
        /// <param name="skill">技能实例</param>
        void AddSkill(ISkill skill);

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="id">技能临时id</param>
        /// <returns>是否使用成功</returns>
        bool UseSkill(AssetLocation id);
    }

    public class SkillSystemImpl : ISkillSystem //TODO:修复前摇时输入其他技能会出问题
    {
        /// <summary>
        /// 有限状态机
        /// </summary>
        private readonly FsmSystem _system;

        /// <summary>
        /// 前摇状态
        /// </summary>
        private readonly WaitState _preUse;

        /// <summary>
        /// 后摇状态
        /// </summary>
        private readonly PostUseSkillState _postUse;

        /// <summary>
        /// 使用技能中状态
        /// </summary>
        private readonly UseSkillState _using;

        /// <summary>
        /// 技能列表
        /// </summary>
        private readonly Dictionary<AssetLocation, ISkill> _skills;

        public event EventHandler<EntitySkillEvent.PreUse> OnPreUseSkill { add => _using.OnPreUseSkill += value; remove => _using.OnPreUseSkill -= value; }
        public event EventHandler<EntitySkillEvent.Using> OnUsingSkill { add => _using.OnUsingSkill += value; remove => _using.OnUsingSkill -= value; }
        public event EventHandler<EntitySkillEvent.PostUse> OnPostUseSkill { add => _postUse.OnPostUseSkill += value; remove => _postUse.OnPostUseSkill -= value; }

        public ISkill UsingSkill { get => _using.UsingSkill; internal set => _using.UsingSkill = value; }

        public SkillSystemImpl()
        {
            _system = new FsmSystem();
            var normal = new NormalState("normal", _system); //普通状态
            _preUse = new WaitState("preUse", _system);
            _using = new UseSkillState("using", _system);
            _postUse = new PostUseSkillState("postUse", _system, this);
            //添加一条过渡链，过渡原因是：有需要使用的技能了
            _system.AddTransition(normal, _preUse, _ => UsingSkill != null);
            //过渡原因：前摇时间结束
            _system.AddTransition(_preUse, _using, info => ((WaitState) info.last).EndTime <= Time.time);
            //过渡原因：技能使用完毕
            _system.AddTransition(_using, _postUse, _ => UsingSkill.IsDone);
            //过渡原因：后摇时间结束
            _system.AddTransition(_postUse, normal, info => ((WaitState) info.last).EndTime <= Time.time);
            _system.NormalState = normal; //设置默认状态
            _system.SetCurrentState(normal); //设置当前状态
            _skills = new Dictionary<AssetLocation, ISkill>();
        }

        public void AddSkill(ISkill skill)
        {
            if (_skills.ContainsKey(skill.Id))
            {
                throw new ArgumentException($"已经添加过的技能：{skill.Id}");
            }

            _skills.Add(skill.Id, skill);
        }

        public bool UseSkill(AssetLocation id)
        {
            if (!_skills.TryGetValue(id, out var skill))
            {
                throw new ArgumentException($"未知技能：{id}");
            }

            if (!skill.CanUse())
            {
                return false;
            }

            UsingSkill = skill;
            _preUse.EndTime = UsingSkill.PerUseTime;
            _postUse.EndTime = UsingSkill.PostUseTime;
            return true;
        }

        public void OnUpdate()
        {
            _system.PerformTransition(); //检查并执行过渡
            _system.CurrentState.OnUpdate();
        }
    }
}