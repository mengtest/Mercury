using System;
using UnityEngine;

[Serializable]
public class MoveCapability : IEntityProperty
{
    public int jumpCount;
    public float jumpCD = 0;
    public int maxJumpCount = 2;
    public float maxJumpCD = 0.2f;
    public bool canMove = true;
    public float gravity = -25f;
    public float runSpeed = 1f;
    public float groundDamping = 20f;
    public float inAirDamping = 5f;
    public float jumpHeight = 1.5f;
    public Vector2 velocity;

    public bool IsJumpReady { get { return jumpCount > 0 && jumpCD <= 0; } }

    public void UpdateJumpCD()
    {
        if (jumpCD > 0)
        {
            jumpCD -= Time.deltaTime;
        }
    }

    public void RecoverJumpCD() { jumpCD = maxJumpCD; }

    public void RecoverJumpCount() { jumpCount = maxJumpCount; }

    public bool TryJump()
    {
        if (!IsJumpReady || !canMove)
        {
            return false;
        }

        jumpCount -= 1;
        jumpCD = maxJumpCD;
        return true;
    }
}