using UnityEngine;

public class StepEnter : MonoBehaviour
{
    public GameObject step;

    public Collider2D _stepCollider2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Consts.TAG_Entity))
        {
            return;
        }

        var e = collision.GetComponent<Entity>();
        if (e.EntityType == EntityType.Player)
        {
            Physics2D.IgnoreCollision(_stepCollider2D, collision, false);
        }
    }
}