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

    public float jumpCheckDistance;

    [Space(10)]
    public bool gliding;
    public Vector2 glideForce;
    Vector2 glideVerticalVelocity;
    public float glideDrag;
    float glideAngle;
    public float minGlideAngle, maxGlideAngle;
    public float glideAngleSpeed;
    public AnimationCurve glideSpeedCurve, glideDragCurve;
    public float glideAcceleration;
    public float gliderVerticalAcceleration;
    public float glideGravity;
    float lastJumpTime;

    public Transform playerSprite;

    [Space(10)]
    public float minCameraSize;
    public float maxCameraSize;
    public float minDistance, maxDistance;
    float distanceFromGround;
    public float cameraSizeSpeed;
    public new Camera camera;
    public Transform background;

    [Space(10)]
    public Animator animator;

    void Update()
    {
        if (!disable)
        {
            Move();
        }
    }

    Vector2 GetGlideVelocity()
    {
        verticalSpeed = Vector2.zero;

        glideAngle = Mathf.Lerp(glideAngle, Input.GetKey(KeyCode.W) ? maxGlideAngle : minGlideAngle, glideAngleSpeed * Time.deltaTime);
        float normalizedAngle = Mathf.InverseLerp(minGlideAngle, maxGlideAngle, glideAngle);
        float xVel = glideForce.x * glideSpeedCurve.Evaluate(normalizedAngle);
        float yVel = glideForce.y * glideDragCurve.Evaluate(normalizedAngle);

        playerSprite.transform.localEulerAngles = new Vector3(0f, 0f, glideAngle);

        glideVerticalVelocity = Vector2.Lerp(glideVerticalVelocity + new Vector2(0f, glideGravity * Time.deltaTime), new Vector2(0f, glideForce.y), glideDragCurve.Evaluate(normalizedAngle));

        return Vector2.Lerp(horizontalSpeed, playerSprite.up * new Vector2(xVel, 0f), glideAcceleration * Time.deltaTime);
    }

    void Move()
    {
        //gliding = !grounded;

        sideInput = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;

        bool walking = sideInput != 0f;
        bool flying = gliding;
        if (animator.GetBool("Walking") != walking)
        {
            animator.SetBool("Walking", walking);
        }
        if (animator.GetBool("Flying") != flying)
        {
            animator.SetBool("Flying", flying);
        }

        // Accelerate/decelerate velocity
        horizontalSpeed = gliding ? GetGlideVelocity() : Vector2.Lerp(horizontalSpeed, sideInput * speed * Vector2.right,
                (sideInput == 0f ? (grounded ? groundDeceleration : airDeceleration) : (grounded ? groundAcceleration : airAcceleration)) * Time.deltaTime);

        if (!gliding)
        {
            glideAngle = Mathf.Lerp(glideAngle, minGlideAngle, glideAngleSpeed * Time.deltaTime);
        }

        bool prevGrounded = grounded;

        // Accelerate y velocity with gravity
        if (grounded)
        {
            // Jump
            gliding = false;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalSpeed = Vector2.up * jumpSpeed;
            }
            else
            {
                verticalSpeed = Vector2.zero;
            }

            playerSprite.transform.localRotation = Quaternion.identity;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;

            if (Time.time - lastJumpTime > 0.2f)
            {
                gliding = true;
            }

            if (Input.GetKeyDown(KeyCode.Space) && Physics2D.Raycast(transform.position, Vector2.down, jumpCheckDistance, GroundMask))
            {
                verticalSpeed = Vector2.up * jumpSpeed;
            }

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    gliding = !gliding;
            //}
        }

        // Set velocity from speed components (in local space)
        velocity = horizontalSpeed + (gliding ? glideVerticalVelocity : verticalSpeed);
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

        hit = Physics2D.Raycast(transform.position, Vector2.down, 1000f, GroundMask);
        distanceFromGround = hit.distance;

        float targetSize = Mathf.Lerp(minCameraSize, maxCameraSize, Mathf.InverseLerp(minDistance, maxDistance, distanceFromGround));
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, cameraSizeSpeed * Time.deltaTime);
        background.transform.localScale = Vector3.one * (camera.orthographicSize / minCameraSize);


        if (!grounded && prevGrounded)
        {
            lastJumpTime = Time.time;
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