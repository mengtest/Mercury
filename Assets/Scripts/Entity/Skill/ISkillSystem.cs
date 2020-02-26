using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    public interface ISkillSystem : IEntitySystem, IUpdatable
    {
        ISkill UsingSkill { get; }

        void AddSkill(ISkill skill);

        bool UseSkill(AssetLocation id);
    }

    public class SkillSystemImpl : ISkillSystem
    {
        private readonly FsmSystem _system;
        private readonly WaitState _preUse;
        private readonly WaitState _postUse;
        private readonly UseSkillState _using;
        private readonly Dictionary<AssetLocation, ISkill> _skills;

        public ISkill UsingSkill { get => _using.UsingSkill; private set => _using.UsingSkill = value; }

        public SkillSystemImpl()
        {
            _system = new FsmSystem();
            var normal = new NormalState("normal", _system);
            _preUse = new WaitState("preUse", _system);
            _using = new UseSkillState("using", _system);
            _postUse = new WaitState("postUse", _system);
            _system.AddTransition(normal, _preUse, _ => UsingSkill != null);
            _system.AddTransition(_preUse, _using, info => ((WaitState) info.last).EndTime <= Time.time);
            _system.AddTransition(_using, _postUse, _ => UsingSkill.IsDone);
            _system.AddTransition(_postUse, normal, info => ((WaitState) info.last).EndTime <= Time.time);
            _system.NormalState = normal;
            _system.SetCurrentState(normal);
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
            //Debug.Log(_system.CurrentState);
            _system.PerformTransition();
            _system.CurrentState.OnUpdate();
        }
    }
}