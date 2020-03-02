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

        SkillState NowState { get; }

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
        private readonly Dictionary<AssetLocation, ISkill> _skills;
        private readonly FsmSystem _system;

        private readonly NormalState _normalState;
        private readonly PreSkillState _preState;
        private readonly UsingSkillState _using;
        private readonly PostSkillState _postState;

        public ISkill UsingSkill { get; private set; }
        public SkillState NowState { get; private set; }

        public event EventHandler<EntitySkillEvent.PreUse> OnPreUseSkill;
        public event EventHandler<EntitySkillEvent.Using> OnUsingSkill;
        public event EventHandler<EntitySkillEvent.PostUse> OnPostUseSkill;

        public SkillSystemImpl()
        {
            _skills = new Dictionary<AssetLocation, ISkill>();
            _system = new FsmSystem();
            _normalState = new NormalState(_system, this);
            _preState = new PreSkillState(_system, this);
            _using = new UsingSkillState(_system, this);
            _postState = new PostSkillState(_system);
            _system.AddTransition(_normalState, _preState, _ => UsingSkill != null);
            _system.AddTransition(_preState, _using, info => ((PreSkillState) info.last).EndTime <= Time.time);
            _system.AddTransition(_using, _postState, _ => UsingSkill.IsDone);
            _system.AddTransition(_postState, _normalState, info => ((PostSkillState) info.last).EndTime <= Time.time);
            _system.NormalState = _normalState;
            _system.SetCurrentState(_normalState);
        }

        public void OnUpdate()
        {
            _system.PerformTransition();
            _system.CurrentState.OnUpdate();
        }

        public void AddSkill(ISkill skill)
        {
            if (_skills.ContainsKey(skill.Id))
            {
                throw new InvalidOperationException($"不能重复添加{skill.Id}");
            }

            _skills.Add(skill.Id, skill);
        }

        public bool UseSkill(AssetLocation id)
        {
            if (!_skills.TryGetValue(id, out var skill))
            {
                return false;
            }

            if (!skill.CanUse())
            {
                return false;
            }

            if (UsingSkill != null)
            {
                _system.SwitchState(_normalState);
            }

            UsingSkill = skill;
            return true;
        }

        public void LeavePost()
        {
            OnPostUseSkill?.Invoke(this, new EntitySkillEvent.PostUse(UsingSkill));
            UsingSkill = null;
            NowState = SkillState.Normal;
        }

        public void EnterPre()
        {
            NowState = SkillState.Pre;
            UsingSkill.OnPreUse();
            OnPreUseSkill?.Invoke(this, new EntitySkillEvent.PreUse(UsingSkill));
            _preState.EndTime = UsingSkill.PerUseTime;
        }

        public void UpdateFrame()
        {
            NowState = SkillState.Using;
            UsingSkill.OnUsing();
            OnUsingSkill?.Invoke(this, new EntitySkillEvent.Using(UsingSkill));
        }

        public void WillLeaveUpdate()
        {
            NowState = SkillState.Post;
            _postState.EndTime = UsingSkill.PostUseTime;
        }
    }
}