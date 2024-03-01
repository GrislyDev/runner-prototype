using System;
using System.Collections;
using UnityEngine;

public enum ChangePosition
{
	LEFT,
	RIGHT
}

public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }

	private Animator _playerAnimator;
	private BoxCollider _topBodyCollider;
	private Rigidbody _playerRb;
	private Vector3 _defaultPos;

	private float _laneOffset = 1;
	private float _laneChangeSpeed = 5;
	private float _xBounds;
	private float _lastLane;
	private float _nextLane;
	private float _lastVectorX;

	private bool _isMoving = false;
	private Coroutine _movingCoroutine;

	#region UNITY_ENGINE
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
			Destroy(gameObject);

		_defaultPos = new Vector3(0, 0.133f, 2.7f);
	}


	private void Start()
	{
		_playerAnimator = GetComponentInChildren<Animator>();
		_topBodyCollider = GetComponent<BoxCollider>();
		_playerRb = GetComponent<Rigidbody>();
	}

	private void LateUpdate()
	{
		StateMachine.Instance.UpdateState();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "StumbleZone")
		{
			if (_isMoving)
			{
				MoveHorizontal(-_lastVectorX);
			}
			else
			{
				_playerRb.AddForce(Vector3.up * 2, ForceMode.Impulse);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "BumpZone")
		{
			SpawnManager.Instance.Speed = 0;
			StateMachine.Instance.ChangeState(StateMachine.Death);
		}
	}
	#endregion

	public Animator GetPlayerAnimator() => _playerAnimator;
	public Rigidbody GetPlayerRigidbody() => _playerRb;
	public void DisableTopBodyCollider() => _topBodyCollider.enabled = false;
	public void EnableTopBodyCollider() => _topBodyCollider.enabled = true;

	public void ChangeLanePosition(ChangePosition direction)
	{
		if (direction == ChangePosition.LEFT)
		{
			if (transform.position.x > -_laneOffset)
			{
				MoveHorizontal(-_laneChangeSpeed);
			}
		}
		else if (direction == ChangePosition.RIGHT)
		{
			if (transform.position.x < _laneOffset)
			{
				MoveHorizontal(_laneChangeSpeed);
			}
		}
	}

	private void MoveHorizontal(float speed)
	{
		if (speed == 0)
			return;

		_lastLane = _nextLane;
		_nextLane += Mathf.Sign(speed) * _laneOffset;

		if (_isMoving)
		{
			StopCoroutine(_movingCoroutine);
			_isMoving = false;
		}

		_movingCoroutine = StartCoroutine(MoveCoroutine(speed));
	}

	IEnumerator MoveCoroutine(float vectorX)
	{
		_isMoving = true;

		while (Mathf.Abs(_lastLane - transform.position.x) < _laneOffset)
		{
			yield return new WaitForFixedUpdate();

			_playerRb.velocity = new Vector3(vectorX, _playerRb.velocity.y, 0);
			_lastVectorX = vectorX;
			_xBounds = Mathf.Clamp(transform.position.x, Mathf.Min(_lastLane, _nextLane), Mathf.Max(_lastLane, _nextLane));
			transform.position = new Vector3(_xBounds, transform.position.y, transform.position.z);
		}
		
		_playerRb.velocity = Vector3.zero;
		transform.position = new Vector3(_nextLane, transform.position.y, transform.position.z);
		_isMoving = false;
	}

	public void Reset()
	{
		_lastLane = default;
		_nextLane = default;
		_lastVectorX = default;

		transform.position = _defaultPos;
	}
}