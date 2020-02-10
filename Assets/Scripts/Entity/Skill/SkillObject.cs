using UnityEngine;

public abstract class SkillObject : MonoBehaviour, IFSMState
{
    public float cd;
    public float lastUse = float.MinValue;

    public AssetLocation RegisterName { get; } = new AssetLocation();
    public abstract FSMSystem System { get; }

    private void OnTriggerEnter2D(Collider2D collision) { OnTriggerEnterEvent(collision); }

    protected virtual void OnTriggerEnterEvent(Collider2D coll) { }

    private void OnTriggerStay2D(Collider2D collision) { OnTriggerStayEvent(collision); }

    protected virtual void OnTriggerStayEvent(Collider2D coll) { }

    private void OnTriggerExit2D(Collider2D collision) { OnTriggerExitEvent(collision); }

    protected virtual void OnTriggerExitEvent(Collider2D coll) { }

    /// <summary>
    /// 是否冷却完毕
    /// </summary>
    protected bool IsCoolDown() { return Time.time - lastUse >= cd; }

    /// <summary>
    /// 刷新冷却时间
    /// </summary>
    protected void RefreshCoolDown() { lastUse = Time.time; }

    protected void EnterStiffness(float time)
    {
        //System.SwitchState(,out var state);
        //state.Duration = time;
    }

    public abstract void Init();

    public abstract bool CanEnter();

    public abstract void OnEnter();

    public abstract void OnUpdate();

    public abstract void OnLeave();
}