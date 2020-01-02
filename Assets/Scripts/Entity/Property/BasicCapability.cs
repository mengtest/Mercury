using System;
/// <summary>
/// 基础属性
/// </summary>
[Serializable]
public class BasicCapability : IEntityProperty
{
	/// <summary>
	/// 法力值
	/// </summary>
	public float manaPoint;
	/// <summary>
	/// 每秒法力回复
	/// </summary>
	public float mpRecoverPerSec;
	/// <summary>
	/// 最大法力值
	/// </summary>
	public float maxManaPoint;
	/// <summary>
	/// 物理攻击力
	/// </summary>
	public float phyAttack;
	/// <summary>
	/// 魔法攻击力
	/// </summary>
	public float magAttack;
	/// <summary>
	/// 物理防御
	/// </summary>
	public float phyDefense;
	/// <summary>
	/// 魔法防御
	/// </summary>
	public float magDefense;
	/// <summary>
	/// 耐力
	/// </summary>
	public int endurancePoint;
	/// <summary>
	/// 精神力
	/// </summary>
	public int spiritPoint;
	/// <summary>
	/// 智力
	/// </summary>
	public int intelligencePoint;
	/// <summary>
	/// 力量
	/// </summary>
	public int strengthPoint;
	/// <summary>
	/// 暴击
	/// </summary>
	public int criticalPoint;
	/// <summary>
	/// 攻速
	/// </summary>
	public int attackSpeedPoint;
	/// <summary>
	/// 吟唱速度
	/// </summary>
	public int chantSpeedPoint;
	/// <summary>
	/// 幸运
	/// </summary>
	public int furtunePoint;
}
