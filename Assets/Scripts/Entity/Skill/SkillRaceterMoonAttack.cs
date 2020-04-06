using UnityEngine;


/// <summary>
/// 三段普通攻击
/// </summary>
public class SkillRaceterMoonAttack : MonoBehaviour
{

    /// <summary>
    /// 平A第一段伤害
    /// </summary>
    [Header("平A第一段伤害")] public float Atk1;
    /// <summary>
    /// 平A第二段伤害
    /// </summary>
    [Header("平A第二段伤害")] public float Atk2;
    /// <summary>
    /// 平A第三段伤害
    /// </summary>
    [Header("平A第三段伤害")] public float Atk3;
        
    /// <summary>
    /// 技能释放者
    /// </summary>
    private GameObject master;

    private void Start()
    {
        master = 
    }
    public void TryUse()
    {
        if (CanUse())
        {

        }
    }

    private bool CanUse()
    {

        return false;
    }
        

    
}