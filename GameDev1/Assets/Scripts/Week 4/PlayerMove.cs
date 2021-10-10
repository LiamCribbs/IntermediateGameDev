using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public const int GroundMask = ~(1 << 3);

    public new Rigidbody2D rigidbody;

    [Space(10)]
    public float speed;
    public float jumpSpeed;
    public float airMoveSpeedMultiplier = 1f;

    [Space(10)]
    public bool grounded;
    public float groundCheckPosition;
    public Vector2 groundCheckSize;

    public float jumpCheckDistance;

    float sideInput;
    bool jumpInput;

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInput()
    {
        sideInput = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }
    }

    void Move()
    {
        // Accelerate y velocity with gravity
        if (grounded)
        {
            // Jump
            if (jumpInput)
            {
                rigidbody.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
            }
        }
        else
        {
            if (jumpInput && Physics2D.Raycast(transform.position, Vector2.down, jumpCheckDistance, GroundMask))
            {
                rigidbody.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
            }
        }

        jumpInput = false;

        rigidbody.velocity = new Vector2(sideInput * speed * Time.fixedDeltaTime * (grounded ? 1f : airMoveSpeedMultiplier), rigidbody.velocity.y);

        // Grounded check
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + transform.up * groundCheckPosition, groundCheckSize, 0f, -transform.up, 0f, GroundMask);
        grounded = hit.collider && rigidbody.velocity.y <= 0f;
    }
}