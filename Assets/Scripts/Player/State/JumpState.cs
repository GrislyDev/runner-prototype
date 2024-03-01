using UnityEngine;

public class JumpState : IState
{
	private const string JUMP_TRIGGER = "hasJumped";
	private const string JUMP_STATE = "Jump";

	private float _jumpForce = 4.5f;

    public void Enter()
	{
		Player.Instance.GetPlayerAnimator().SetTrigger(JUMP_TRIGGER);
		Player.Instance.GetPlayerRigidbody().AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
		SwipeController.SwipeEvent += SwipeDetection;
	}

	public void Execute()
	{
		if (!Player.Instance.GetPlayerAnimator().GetCurrentAnimatorStateInfo(0).IsName(JUMP_STATE))
			StateMachine.Instance.ChangeState(StateMachine.Running);
	}

	public void Exit()
	{
		SwipeController.SwipeEvent -= SwipeDetection;
	}

	private void SwipeDetection(Vector2 swipe)
	{
		if (swipe.y < 0)
			StateMachine.Instance.ChangeState(StateMachine.Roll);


		if (swipe.x > 0)
		{
			if (Player.Instance.transform.position.x < 1)
			{
				Player.Instance.ChangeLanePosition(ChangePosition.RIGHT);
			}
		}
		else if (swipe.x < 0)
		{
			if (Player.Instance.transform.position.x > -1)
			{
				Player.Instance.ChangeLanePosition(ChangePosition.LEFT);
			}
		}
	}
}