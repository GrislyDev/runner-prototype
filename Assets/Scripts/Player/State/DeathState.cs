public class DeathState : IState
{
	private const string DEATH_STATE = "Death";


	public void Enter()
	{
		GameOverHandler();
		ScoreManager.Instance.StopCount();
		GameUIHandler.Instance.OpenGameOverPanel();
		Player.Instance.GetPlayerAnimator().Play(DEATH_STATE);
	}

	public void Execute()
	{
	}

	public void Exit()
	{
	}

	private void GameOverHandler()
	{
		AdsManager.Instance.LoadRewardedAd();
		FirebaseManager.Instance.SaveData();
	}
}