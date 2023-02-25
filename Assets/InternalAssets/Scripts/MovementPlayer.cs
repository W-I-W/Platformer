using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player
{
	[SerializeField] private float _speed;

	private Vector2 _move;
	private float _currentHorizontalSpeed;

	private RaycastHit2D _isGrounded;

	private bool IsConflictCollision => _isGrounded.point.y > _isGrounded.collider.bounds.max.y;

	private float OnHorizontalSpeedTowards(float timeDelta) => (IsGrounded) ? _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, _move.x, timeDelta) : 0;

	private void Jumping()
	{
		if (IsGrounded && _jumpKeyPressed)
			_physics.AddForce(new Vector2(0, _jumpForge));
	}

	private void MovementX(float speed)
	{
		if (_move == Vector2.zero || !IsGrounded) return;
		_physics.velocity = new Vector2(speed, 0) * _speed;
	}

	public virtual void OnMovement()
	{
		float horizontalSpeed = OnHorizontalSpeedTowards(Time.fixedDeltaTime);
		Jumping();
		MovementX(horizontalSpeed);
	}





}
