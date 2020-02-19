public class SkillRaceterMoonAttack : AbstractSkill
{
    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterMoonAttack;

    public SkillRaceterMoonAttack(ISkillable user) : base(user) { }

    public override void Init() { }

    public override bool CanEnter() { throw new System.NotImplementedException(); }

    public override void OnEnter() { }

    public override void OnUpdate() { }

    public override void OnLeave() { }
}