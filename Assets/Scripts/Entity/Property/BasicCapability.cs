/// <summary>
/// 基础属性
/// </summary>
public class BasicCapability : IEntityProperty
{
	/// <summary>
	/// 法力值
	/// </summary>
	public float ManaPoint { get; set; }
	/// <summary>
	/// 每秒法力回复
	/// </summary>
	public float MpRecoverPerSec { get; set; }
	/// <summary>
	/// 最大法力值
	/// </summary>
	public float MaxManaPoint { get; set; }
	/// <summary>
	/// 物理攻击力
	/// </summary>
	public float PhyAttack { get; set; }
	/// <summary>
	/// 魔法攻击力
	/// </summary>
	public float MagAttack { get; set; }
	/// <summary>
	/// 物理防御
	/// </summary>
	public float PhyDefense { get; set; }
	/// <summary>
	/// 魔法防御
	/// </summary>
	public float MagDefense { get; set; }
	/// <summary>
	/// 耐力
	/// </summary>
	public int EndurancePoint { get; set; }
	/// <summary>
	/// 精神力
	/// </summary>
	public int SpiritPoint { get; set; }
	/// <summary>
	/// 智力
	/// </summary>
	public int IntelligencePoint { get; set; }
	/// <summary>
	/// 力量
	/// </summary>
	public int StrengthPoint { get; set; }
	/// <summary>
	/// 暴击
	/// </summary>
	public int CriticalPoint { get; set; }
	/// <summary>
	/// 攻速
	/// </summary>
	public int AttackSpeedPoint { get; set; }
	/// <summary>
	/// 吟唱速度
	/// </summary>
	public int ChantSpeedPoint { get; set; }
	/// <summary>
	/// 幸运
	/// </summary>
	public int FurtunePoint { get; set; }
}
