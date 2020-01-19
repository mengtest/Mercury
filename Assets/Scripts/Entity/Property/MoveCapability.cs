using System;
using UnityEngine;

[Serializable]
public class MoveCapability : IEntityProperty
{
	public bool isJumpReady = true;
	public int jumpCount;
	public float moveSpeed = 15;
	public float jumpSpeed = 12;
	public float jumpCD = 0;

	public int maxJumpCount = 2;
	public float maxJumpCD = 0.2f;
	public bool canMove = true;

	public bool IsJumpReady
	{
		get
		{
			return jumpCount > 0 && jumpCD <= 0;
		}
	}

	public void UpdateJumpCD()
	{
		if (jumpCD > 0)
		{
			jumpCD -= Time.deltaTime;
		}
	}

	public void RecoverJumpCD()
	{
		jumpCD = maxJumpCD;
	}

	public void RecoverJumpCount()
	{
		jumpCount = maxJumpCount;
	}

	public bool TryJump()
	{
		if (IsJumpReady)
		{
			jumpCount -= 1;
			jumpCD = maxJumpCD;
			return true;
		}
		return false;
	}
}
