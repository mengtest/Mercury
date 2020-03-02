using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 技能系统
    /// </summary>
    public interface ISkillSystem : IEntitySystem
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

    public class SkillSystemImpl : MonoBehaviour, ISkillSystem
    {
        public SkillState nowState;

        private Dictionary<AssetLocation, ISkill> _skills;
        private FsmSystem _system;
        private NormalState _normalState;
        private PreSkillState _preState;
        private UsingSkillState _using;
        private PostSkillState _postState;

        public ISkill UsingSkill { get; private set; }
        public SkillState NowState => nowState;

        public event EventHandler<EntitySkillEvent.PreUse> OnPreUseSkill;
        public event EventHandler<EntitySkillEvent.Using> OnUsingSkill;
        public event EventHandler<EntitySkillEvent.PostUse> OnPostUseSkill;

        private void Awake()
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

        private void Update()
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
                UsingSkill.OnPostUse();
                LeavePost();
                _system.SwitchState(_normalState);
            }

            UsingSkill = skill;
            return true;
        }

        public void LeavePost()
        {
            OnPostUseSkill?.Invoke(this, new EntitySkillEvent.PostUse(UsingSkill));
            UsingSkill = null;
            nowState = SkillState.Normal;
        }

        public void EnterPre()
        {
            nowState = SkillState.Pre;
            UsingSkill.OnPreUse();
            OnPreUseSkill?.Invoke(this, new EntitySkillEvent.PreUse(UsingSkill));
            _preState.EndTime = UsingSkill.PerUseTime;
        }

        public void UpdateFrame()
        {
            nowState = SkillState.Using;
            UsingSkill.OnUsing();
            OnUsingSkill?.Invoke(this, new EntitySkillEvent.Using(UsingSkill));
        }

        public void WillLeaveUpdate()
        {
            nowState = SkillState.Post;
            _postState.EndTime = UsingSkill.PostUseTime;
        }
    }
}