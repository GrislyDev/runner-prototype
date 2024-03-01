using UnityEngine;

public class RollState : IState
{
	private const string ROLL_TRIGGER = "hasRolled";
	private const string ROLL_STATE = "Roll";

	private float _rollForce = 4.5f;

    public void Enter()
	{
		SwipeController.SwipeEvent += SwipeDetection;
		Player.Instance.GetPlayerAnimator().SetTrigger(ROLL_TRIGGER);
		Player.Instance.DisableTopBodyCollider();
		Player.Instance.GetPlayerRigidbody().AddForce(Vector3.down * _rollForce, ForceMode.Impulse);
	}

	public void Execute()
	{
		if (!Player.Instance.GetPlayerAnimator().GetCurrentAnimatorStateInfo(0).IsName(ROLL_STATE))
		{
			StateMachine.Instance.ChangeState(StateMachine.Running);
		}
	}

	public void Exit()
	{
		SwipeController.SwipeEvent -= SwipeDetection;
		Player.Instance.EnableTopBodyCollider();
	}

	private void SwipeDetection(Vector2 swipe)
	{
		if (swipe.y > 0)
			StateMachine.Instance.ChangeState(StateMachine.Jump);

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