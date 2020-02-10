using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public interface ISkillable
{
    FSMSystem SkillFsmSystem { get; }

    T GetProperty<T>() where T : class, IEntityProperty;

    void AddSkill(IFSMState skill);

    bool RemoveSkill(string skillName);

    void UseSkill(string skillName);

    void UseSkill(string skillName, out IFSMState skill);

    void OnUpdateSkills();
}