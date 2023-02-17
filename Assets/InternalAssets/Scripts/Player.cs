using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
	[SerializeField] private float _speed;
	[SerializeField] private LayerMask _layerGround;
	[SerializeField] private int _jumpForge = 1;

	private Rigidbody2D _physics;
	private Vector2 _move;
	private float _depth = 0;
	private RaycastHit2D _isGrounded;

	private int _jumpClick = 0;

	private readonly Vector2 BoxSize = new(1f, 0.02f);
	private readonly Vector2 ShiftRayBox = new(0, -0.5f);

	private const float PlatformTimeRotation = 0.4f;
	private void Start()
	{
		_physics = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		Movement();
		Raycast();
	}

	private void Movement()
	{
		_isGrounded = Physics2D.BoxCast((Vector2)transform.position + ShiftRayBox, BoxSize, 0, Vector2.zero, 1, _layerGround);
		bool noConfictCollition = false;
		if (_isGrounded)
			noConfictCollition = _isGrounded.point.y > _isGrounded.collider.bounds.max.y;

		if (_isGrounded && _jumpClick > 0 && noConfictCollition)
		{
			_physics.AddForce(new Vector2(0, _jumpForge));
			_jumpClick = 0;
		}

		if (_move == Vector2.zero || !_isGrounded) return;
		if (!noConfictCollition) return;
		_physics.velocity = _move * _speed * Vector2.right;
	}

	private void Raycast()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, _physics.velocity, 4, _layerGround);
		if (hit == false) return;
		_depth = hit.transform.position.z;
		transform.position = new Vector3(transform.position.x, transform.position.y, _depth);
		Debug.DrawLine((Vector2)transform.position, hit.point, Color.red);
	}

	private void OnMove(InputValue value) => _move = (value.Get<Vector2>());

	private void OnJump(InputValue value) => _jumpClick = Convert.ToInt32(value.Get());

	private void OnDown(InputValue value) => StartCoroutine(PlatformRotation());

	private IEnumerator PlatformRotation()
	{
		if (_isGrounded == false) yield break;

		bool isPlatform = _isGrounded.transform.TryGetComponent(out PlatformEffector2D platform);

		if (!isPlatform) yield break;

		platform.rotationalOffset = 180;
		yield return new WaitForSeconds(PlatformTimeRotation);
		platform.rotationalOffset = 0;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position + (Vector3)ShiftRayBox, BoxSize);
	}
}
