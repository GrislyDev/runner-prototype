using UnityEngine;

public class IdleState : IState
{
	private const string IDLE_STATE = "Idle";


    public void Enter()
	{
		Player.Instance.GetPlayerAnimator().Play(IDLE_STATE);
	}

	public void Execute()
	{
	}

	public void Exit()
	{
	}
}