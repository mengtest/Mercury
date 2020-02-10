public static class Consts
{
    public static readonly string PREFAB_SE_SkillRaceterShadowStrike = "SkillRaceterShadowStrike";
    public static readonly string PREFAB_SE_SkillRaceterBladeWave = "SkillRaceterBladeWave";
    public static readonly string PREFAB_SE_SkillRaceterHasaki = "SkillRaceterHasaki";
    public static readonly string PREFAB_SE_SkillRaceterSwallowFlip = "SkillRaceterSwallowFlip";
    public static readonly string PREFAB_SE_SkillRaceterFlashCut = "SkillRaceterFlashCut";

    public static readonly string TAG_SpecialEffect = "SpecialEffect";
    public static readonly string TAG_Step = "Step";
    public static readonly string TAG_StepCross = "StepCross";
    
    public static readonly string Mercury = "mercury";
    public static readonly string Entity = "entity";
    public static readonly string Skill = "skill";

    public static readonly AssetLocation SkillNormal = new AssetLocation(Mercury, Skill, "normal");
    public static readonly AssetLocation SkillStiffness = new AssetLocation(Mercury, Skill, "stiffness");

    public static readonly AssetLocation EntityWoodMan = new AssetLocation(Mercury, Entity, "wood_man");
    public static readonly AssetLocation EntityFlightProp = new AssetLocation(Mercury, Entity, "flight_prop");
    public static readonly AssetLocation EntityRaceter = new AssetLocation(Mercury, Entity, "raceter");

    public static readonly string BUFF_WindMark = "Mercury:WindMark";
    public static readonly string BUFF_Heal = "Mercury:Heal";
}