using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all data for the player monobehaviour instance
/// </summary>
public class Player : MonoBehaviour
{

    public const int GroundMask = ~(1 << 3);

    public new BoxCollider2D collider;

    readonly Collider2D[] collisions = new Collider2D[16];
    int numCollisions;

    bool disable;

    Vector2 velocity;
    float sideInput;
    Vector2 horizontalSpeed;
    Vector2 verticalSpeed;

    [Space(10)]
    public float speed;
    public float jumpSpeed;

    [Space(10)]
    public float jetpackForce;
    public float jetpackImpulseForce;

    [Space(10)]
    public float groundAcceleration;
    public float groundDeceleration;
    public float airAcceleration;
    public float airDeceleration;

    [Space(10)]
    public Vector2 gravity;

    [Space(10)]
    public bool grounded;
    //public bool colliderBelow;
    public float maxGroundAngle;
    public float groundCheckPosition;
    public Vector2 groundCheckSize;
    public float groundAngleCheckSize;

    void Start()
    {

    }

    void OnDestroy()
    {

    }

    void Update()
    {
        if (!disable)
        {
            Move();
        }
    }

    void Move()
    {
        sideInput = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;

        // Accelerate/decelerate velocity
        horizontalSpeed = Vector2.Lerp(horizontalSpeed, sideInput * speed * Vector2.right,
                (sideInput == 0f ? (grounded ? groundDeceleration : airDeceleration) : (grounded ? groundAcceleration : airAcceleration)) * Time.deltaTime);

        // Accelerate y velocity with gravity
        if (grounded)
        {
            // Jump
            verticalSpeed = Input.GetKeyDown(KeyCode.Space) ? Vector2.up * jumpSpeed : Vector2.zero;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }

        // Set velocity from speed components (in local space)
        velocity = horizontalSpeed + verticalSpeed;
        //velocity = transform.right * xSpeed + transform.up * ySpeed;

        // Set initial transform
        transform.position += (Vector3)velocity * Time.deltaTime;
        Physics2D.SyncTransforms();

        float TARGET_ANGLE = 0f;

        GetCollisions(transform.position, TARGET_ANGLE);

        // Resolve collisions
        for (int i = 0; i < numCollisions; i++)
        {
            ColliderDistance2D hitDistance = collisions[i].Distance(collider);

            // Make sure we're still overlapped - resolving one collision may inadvertantly resolve others
            if (hitDistance.isOverlapped && !collisions[i].isTrigger)
            {
                Vector3 hitDirection = hitDistance.pointA - hitDistance.pointB;
                transform.position += hitDirection;
                Physics2D.SyncTransforms();

                Vector3 localHitDirection = transform.InverseTransformVector(hitDirection);
                //print(localHitDirection.y);
                if (localHitDirection.y < 0.1f)
                {
                    //verticalSpeed = GameManager.zero;
                }
            }
        }

        Vector2 downDirection = Quaternion.Euler(0f, 0f, TARGET_ANGLE) * -transform.up;

        // Grounded check
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + transform.up * groundCheckPosition, groundCheckSize, TARGET_ANGLE, downDirection, 0f, GroundMask);
        if (hit.collider && velocity.y <= 0f)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        // Check for collider underneath us
        //hit = Physics2D.Raycast(transform.position + transform.up * groundCheckPosition, Vector2.down, groundAngleCheckSize, GroundMask);
        //if (hit.collider)
        //{
        //    float normalAngle = -Vector2.SignedAngle(hit.normal, Vector2.up);
        //    if (normalAngle > -maxGroundAngle && normalAngle < maxGroundAngle && ySpeed <= 0f)
        //    {
        //        TARGET_ANGLE = normalAngle;
        //        colliderBelow = true;
        //    }
        //}
        //else
        //{
        //    colliderBelow = false;
        //}
    }

    //public bool Falling
    //{
    //    get
    //    {
    //        return y
    //    }
    //}

    void GetCollisions(Vector2 position, float angle)
    {
        numCollisions = Physics2D.OverlapBoxNonAlloc(position, collider.size, angle, collisions, GroundMask);
    }
}