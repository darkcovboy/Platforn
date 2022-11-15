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

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(LayerMask);
        contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        float getAxis = Input.GetAxis("Horizontal");
        targetVelocity = new Vector2(getAxis, 0);

        if (getAxis == 1  || getAxis == -1)
        {
            _animator.SetBool(IsRunningAnimate, true);
        }
        else
        {
            _animator.SetBool(IsRunningAnimate, false);
        }

        if (Input.GetKey(KeyCode.Space) && grounded)
            Velocity.y = 5;
    }

    private void FixedUpdate()
    {
        Velocity += GravityModifier * Physics2D.gravity * Time.deltaTime;
        Velocity.x = targetVelocity.x;
        grounded = false;

        Vector2 deltaPosition = Velocity*Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement_Go(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement_Go(move, true);
    }


    private void Movement_Go(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

            hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;

                if (currentNormal.y > MinGroundNormalY)
                {
                    grounded = true;

                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(Velocity, currentNormal);

                if (projection < 0)
                {
                    Velocity = Velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }
    /*
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           _rigidBody2D.velocity = new Vector2(_rigidBody2D.velocity.x, _jumpForce);
        }
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _animator.SetBool(IsRunningAnimate, true);
            _sprite.flipX = false;
            transform.position += _speed * Time.deltaTime * new Vector3(1, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _animator.SetBool(IsRunningAnimate, true);
            _sprite.flipX = true;
            transform.position += _speed * Time.deltaTime * new Vector3(-1, 0, 0);
        }
        else
        {
            _animator.SetBool(IsRunningAnimate, false);
        }
    }
    */
}
