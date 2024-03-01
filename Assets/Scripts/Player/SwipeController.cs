using System.Collections;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
	#region Events
	public delegate void OnSwipeInput(Vector2 direction);
	public static event OnSwipeInput SwipeEvent;
	#endregion

	private Vector2 _tapPosition;
	private Vector2 _swipeDelta;
	private Coroutine _swipeCheckCoroutine;

	private float _defaultDeadZone = 80f;
	private float _resolutionMultiplier = 1f;
	private float _deadZone;

	private bool _isSwiping;

	private void Start()
	{
		Resolution currentResolution = Screen.currentResolution;
		_resolutionMultiplier = (float)currentResolution.width / 1920f;
		_deadZone = _defaultDeadZone * _resolutionMultiplier;

		_swipeCheckCoroutine = StartCoroutine(SwipeCheckCoroutine());
	}

	private void OnDestroy()
	{
		StopCoroutine(_swipeCheckCoroutine);
	}
	private IEnumerator SwipeCheckCoroutine()
	{
		var delay = new WaitForEndOfFrame();

		while (true)
		{
			yield return delay;
			UpdateSwipe();
		}
	}

	private void UpdateSwipe()
	{
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				_isSwiping = true;
				_tapPosition = Input.GetTouch(0).position;
			}
			else if (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				ResetSwipe();
			}
		}

		if (Input.touchCount > 0)
			CheckSwipe();
	}

	private void CheckSwipe()
	{
		_swipeDelta = Vector2.zero;

		if (_isSwiping)
		{
			_swipeDelta = Input.GetTouch(0).position - _tapPosition; ;
		}

		if (_swipeDelta.magnitude > _deadZone)
		{
			if (Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y))
			{
				SwipeEvent?.Invoke(_swipeDelta.x > 0 ? Vector2.right : Vector2.left);
			}
			else
			{
				SwipeEvent?.Invoke(_swipeDelta.y > 0 ? Vector2.up : Vector2.down);
			}

			ResetSwipe();
		}
	}

	private void ResetSwipe()
	{
		_isSwiping = false;
		_tapPosition = Vector2.zero;
		_swipeDelta = Vector2.zero;
	}
}