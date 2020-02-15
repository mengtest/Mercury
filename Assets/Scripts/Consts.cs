public static class Consts
{
    public static readonly string Mercury = "mercury";
    public static readonly string Entity = "entity";
    public static readonly string Skill = "skill";
    public static readonly string Buff = "buff";
    public static readonly string AnimClip = "anim_clip";

    #region Prefabs

    public static readonly AssetLocation PrefabSkillRaceterSwallowFlip =
        new AssetLocation(Mercury, Skill, "raceter_swallow_flip");

    #endregion

    #region Skills

    public static readonly AssetLocation SkillNormal = new AssetLocation(Mercury, Skill, "normal");
    public static readonly AssetLocation SkillStiffness = new AssetLocation(Mercury, Skill, "stiffness");

    public static readonly AssetLocation SkillRaceterShadowStrike =
        new AssetLocation(Mercury, Skill, "raceter_shadow_strike");

    public static readonly AssetLocation SkillRaceterIaiAndSwallowFlip =
        new AssetLocation(Mercury, Skill, "raceter_iai_swallow_flip");

    public static readonly AssetLocation SkillRaceterBladeWave =
        new AssetLocation(Mercury, Skill, "raceter_blade_wave");

    public static readonly AssetLocation SkillRaceterFlashCut = new AssetLocation(Mercury, Skill, "raceter_flash_cut");

    public static readonly AssetLocation SkillRaceterWindPace = new AssetLocation(Mercury, Skill, "raceter_Wind_pace");

    #endregion

    #region Entities

    public static readonly AssetLocation EntityWoodMan = new AssetLocation(Mercury, Entity, "wood_man");
    public static readonly AssetLocation EntityFlightProp = new AssetLocation(Mercury, Entity, "flight_prop");
    public static readonly AssetLocation EntityRaceter = new AssetLocation(Mercury, Entity, "raceter");

    #endregion

    #region Buffs

    public static readonly AssetLocation BuffWindMark = new AssetLocation(Mercury, Buff, "wind_mark");
    public static readonly AssetLocation BuffHeal = new AssetLocation(Mercury, Buff, "heal");
    public static readonly AssetLocation BuffWindPace = new AssetLocation(Mercury, Buff, "wind_pace");

    #endregion

    public static string GetAnimClip(string clipName) { return AnimClip + "." + clipName; }
}