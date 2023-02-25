using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public partial class Player : MonoBehaviour
{
	[SerializeField] private LayerMask _layerGround;
	[SerializeField] private int _jumpForge = 1;
	[SerializeField] private float _speedMoveCamera = 1;

	private Rigidbody2D _physics;
	private float _currentDepth = 0;
	private float _depth = 0;

	private bool _jumpKeyPressed = false;

	private readonly Vector2 BoxSize = new(1f, 0.02f);
	private readonly Vector2 ShiftRayBox = new(0, -0.5f);

	private const float PlatformTimeRotation = 0.44f;

	public virtual RaycastHit2D IsGroundedHit => _isGrounded = Physics2D.BoxCast((Vector2)transform.position + ShiftRayBox, BoxSize, 0, Vector2.zero, 1, _layerGround);
	public bool IsGrounded { get; private set; }


	private void Start()
	{
		_physics = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		IsGrounded = IsGroundedHit && IsConflictCollision;

		OnMovement();
		Raycast();
	}

	private void Raycast()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, _physics.velocity, 4, _layerGround);
		_currentDepth = Mathf.MoveTowards(_currentDepth, _depth, Time.fixedDeltaTime * _speedMoveCamera);
		transform.position = new Vector3(transform.position.x, transform.position.y, _currentDepth);

		if (hit == false) return;
		_depth = hit.transform.position.z;
		Debug.DrawLine((Vector2)transform.position, hit.point, Color.red);
	}

	private void OnMove(InputValue value) => _move = (value.Get<Vector2>());

	private void OnJump(InputValue value) => _jumpKeyPressed = value.isPressed;

	private void OnDown(InputValue value) => StartCoroutine(PlatformRotation());

	private IEnumerator PlatformRotation()
	{
		if (IsGrounded == false) yield break;

		bool isPlatform = IsGroundedHit.transform.TryGetComponent(out PlatformEffector2D platform);

		if (!isPlatform) yield break;

		platform.useColliderMask = true;
		yield return new WaitForSeconds(PlatformTimeRotation);
		platform.useColliderMask = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position + (Vector3)ShiftRayBox, BoxSize);
	}
}
