using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public interface ISkillable
{
    FSMSystem SkillFsmSystem { get; }

    List<AssetReference> SkillObjects { get; }

    void AddSkill(IFSMState skill);

    bool RemoveSkill<T>() where T : class, IFSMState;

    void UseSkill<T>() where T : class, IFSMState;

    void UseSkill<T>(out T skill) where T : class, IFSMState;

    void OnUpdateSkills();
}