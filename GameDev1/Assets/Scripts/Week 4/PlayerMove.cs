using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public const int GroundMask = ~(1 << 3);

    public static PlayerMove instance;

    public new Rigidbody2D rigidbody;
    public Transform sprite;

    public new BoxCollider2D collider;
    readonly Collider2D[] collisions = new Collider2D[16];
    readonly RaycastHit2D[] groundCheckResults = new RaycastHit2D[8];
    int numCollisions;

    [Space(10)]
    public float gravity;

    [Space(10)]
    public float speed;
    public float jumpSpeed;
    public float airMoveSpeedMultiplier = 1f;

    [Space(10)]
    public float groundAcceleration;
    public float airAcceleration;
    public float groundDeceleration;
    public float airDeceleration;

    public Vector2 velocity;

    [Space(10)]
    public float groundCheckPosition;
    public Vector2 groundCheckSize;
    public bool grounded;

    float sideInput;
    public bool jumpInput;

    int _disableRequests;
    public int DisableRequests
    {
        get => _disableRequests;
        set
        {
            if (value <= 0)
            {
                _disableRequests = 0;
                velocity = Vector2.zero;
            }
            else
            {
                _disableRequests = value;
            }
        }
    }
    public bool Enabled
    {
        get => _disableRequests == 0;
    }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Enabled)
        {
            GetInput();
        }
    }

    void FixedUpdate()
    {
        if (Enabled)
        {
            Move();
        }
    }

    void GetInput()
    {
        sideInput = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            float scale = sprite.localScale.y;
            sprite.localScale = new Vector3(scale, scale, scale);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            float scale = sprite.localScale.y;
            sprite.localScale = new Vector3(-scale, scale, scale);
        }
    }

    void Move()
    {
        // Grounded check
        int hits = Physics2D.BoxCastNonAlloc(transform.position + Vector3.up * groundCheckPosition, groundCheckSize, 0f, Vector2.down, groundCheckResults, 0f, GroundMask);
        grounded = false;
        for (int i = 0; i < hits; i++)
        {
            if (groundCheckResults[i].collider && !groundCheckResults[i].collider.isTrigger)
            {
                grounded = true;
                break;
            }
        }

        // Accelerate/decelerate velocity
        velocity.x = Mathf.Lerp(velocity.x, sideInput * speed,
                (sideInput == 0f ? (grounded ? groundDeceleration : airDeceleration) : (grounded ? groundAcceleration : airAcceleration)) * Time.deltaTime);

        // Accelerate y velocity with gravity
        if (grounded)
        {
            // Jump
            if (jumpInput && grounded)
            {
                velocity.y = jumpSpeed;
            }
            else
            {
                velocity.y = -0.5f;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Set initial transform
        transform.position += (Vector3)(velocity * Time.deltaTime);
        Physics2D.SyncTransforms();

        GetCollisions(transform.position);

        // Resolve collisions
        for (int i = 0; i < numCollisions; i++)
        {
            if (collisions[i].isTrigger)
            {
                continue;
            }

            ColliderDistance2D hitDistance = collisions[i].Distance(collider);

            // Make sure we're still overlapped - resolving one collision may inadvertantly resolve others
            if (hitDistance.isOverlapped && !collisions[i].isTrigger)
            {
                Vector3 hitDirection = hitDistance.pointA - hitDistance.pointB;
                transform.position += hitDirection;
                Physics2D.SyncTransforms();

                //Vector3 localHitDirection = transform.InverseTransformVector(hitDirection);
                ////print(localHitDirection.y);
                //if (localHitDirection.y < 0.1f)
                //{
                //    ///verticalSpeed = GameManager.zero;
                //}
            }
        }

        jumpInput = false;
    }

    void GetCollisions(Vector2 position)
    {
        numCollisions = Physics2D.OverlapBoxNonAlloc(position, collider.size, 0f, collisions, GroundMask);
    }
}