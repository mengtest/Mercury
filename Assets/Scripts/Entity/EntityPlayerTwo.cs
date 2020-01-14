using UnityEngine;

public class EntityPlayerTwo : Entity
{
    private CharacterController _controller;

    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
    }

    protected override void Update()
    {
        base.Update();
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _controller.Move(new Vector3(horizontal, vertical, 0) * 0.2f);
        //_controller.isGrounded
    }
}
