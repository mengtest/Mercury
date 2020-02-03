using UnityEngine;

public class SkillObject : MonoBehaviour
{
    public float cd;
    public float lastUse = float.MinValue;

    public Collider2D Contact { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision) { OnEntityTrigger(collision); }

    private void OnTriggerStay2D(Collider2D collision) { OnEntityTrigger(collision); }

    private void OnTriggerExit2D(Collider2D collision) { Contact = null; }

    private void OnEntityTrigger(Collider2D collision)
    {
        if (collision.CompareTag(Consts.TAG_Entity))
        {
            Contact = collision;
        }
    }

    /// <summary>
    /// 是否冷却完毕
    /// </summary>
    protected bool IsCoolDown() { return Time.time - lastUse >= cd; }

    /// <summary>
    /// 刷新冷却时间
    /// </summary>
    protected void RefreshCoolDown() { lastUse = Time.time; }
}