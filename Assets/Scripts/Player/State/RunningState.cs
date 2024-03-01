using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class RunningState : IState
{
	private const string RUNNING_BOOL = "isRunning";

    public void Enter()
	{
		Player.Instance.GetPlayerAnimator().SetBool(RUNNING_BOOL, true);
		SwipeController.SwipeEvent += SwipeDetection;
	}

	public void Execute()
	{
	}

	public void Exit()
	{
		SwipeController.SwipeEvent -= SwipeDetection;
		Player.Instance.GetPlayerAnimator().SetBool(RUNNING_BOOL, false);
	}

	private void SwipeDetection(Vector2 swipe)
	{
		if (swipe.y > 0)
			StateMachine.Instance.ChangeState(StateMachine.Jump);
		else if (swipe.y < 0)
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