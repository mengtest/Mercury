using System;

[Serializable]
public class MoveCapability : IEntityProperty
{
	public bool doubleJumpReady;
	public float moveSpeed = 2;
	public float nowSpeed = 0;
	public float maxSpeed = 15;
	public float jumpSpeed = 0;
	public float doubleJumpColdDownTime = 0;
}
