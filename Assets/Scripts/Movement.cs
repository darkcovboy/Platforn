using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private const string isRunningAnimate = "isRunning";

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    private Animator _animator;
    private SpriteRenderer _sprite;
    private Rigidbody2D _rigidBody2D;
    private Vector2 _moveVector2;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Run();
        Jump();
    }

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
            _animator.SetBool(isRunningAnimate, true);
            _sprite.flipX = false;
            transform.position += _speed * Time.deltaTime * new Vector3(1, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _animator.SetBool(isRunningAnimate, true);
            _sprite.flipX = true;
            transform.position += _speed * Time.deltaTime * new Vector3(-1, 0, 0);
        }
        else
        {
            _animator.SetBool(isRunningAnimate, false);
        }
    }
}
