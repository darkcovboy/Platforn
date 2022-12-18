using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private const string IsRunningAnimate = "isRunning";

    public float MinGroundNormalY = .65f;
    public float GravityModifier = 1f;
    public Vector2 Velocity;
    public LayerMask LayerMask;
    public float Speed = 5f;

    protected Vector2 TargetVelocity;
    protected bool Grounded;
    protected Vector2 GroundNormal;
    protected Rigidbody2D Rb2d;
    protected ContactFilter2D ContactFilter;
    protected RaycastHit2D[] HitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> HitBufferList = new List<RaycastHit2D>(16);
    protected Animator Animator;
    protected SpriteRenderer Sprite;

    protected const float MinMoveDistance = 0.001f;
    protected const float ShellRadius = 0.01f;

    private void OnEnable()
    {
        Rb2d = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ContactFilter.useTriggers = false;
        ContactFilter.SetLayerMask(LayerMask);
        ContactFilter.useLayerMask = true;
    }

    private void Update()
    {
        float getAxis = Input.GetAxis("Horizontal");
        TargetVelocity = new Vector2(getAxis, 0);

        if (getAxis != 0)
        {
            if (getAxis == 1)
                Sprite.flipX = false;
            else
                Sprite.flipX = true;
            Animator.SetBool(IsRunningAnimate, true);
        }
        else
        {
            Animator.SetBool(IsRunningAnimate, false);
        }

        if (Input.GetKey(KeyCode.Space) && Grounded)
            Velocity.y = 10;
    }

    private void FixedUpdate()
    {
        Velocity += GravityModifier * Physics2D.gravity * Time.deltaTime;
        Debug.Log(Velocity);
        Velocity.x = TargetVelocity.x * Speed;
        Debug.Log(Velocity.y);
        Grounded = false;

        Vector2 deltaPosition = Velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(GroundNormal.y, -GroundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;

        Move(move, false);

        move = Vector2.up * deltaPosition.y;

        Move(move, true);
    }


    private void Move(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > MinMoveDistance)
        {
            int count = Rb2d.Cast(move, ContactFilter, HitBuffer, distance + ShellRadius);

            HitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                HitBufferList.Add(HitBuffer[i]);
            }

            for (int i = 0; i < HitBufferList.Count; i++)
            {
                Vector2 currentNormal = HitBufferList[i].normal;

                if (currentNormal.y > MinGroundNormalY)
                {
                    Grounded = true;

                    if (yMovement)
                    {
                        GroundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(Velocity, currentNormal);

                if (projection < 0)
                {
                    Velocity = Velocity - projection * currentNormal;
                }

                float modifiedDistance = HitBufferList[i].distance - ShellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        Rb2d.position = Rb2d.position + move.normalized * distance;
    }
}
