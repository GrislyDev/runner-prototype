using UnityEngine;

public class GameManager : MonoBehaviour
{
	// Singleton
	public static GameManager Instance { get; private set; }

	#region GLOBAL-VARIABLES
	public static string CurrentUser { get; set; }
	#endregion

	[SerializeField] private float _startLevelSpeed = 4f;
	private float _currentLevelSpeed;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
		}
	}

	private void OnEnable()
	{
		IronSourceRewardedVideoEvents.onAdRewardedEvent += ContinueGame;
		ScoreManager.Instance.PlayerScoreReachedThreshold += PlayerScoreReachedThresholdHandler;
	}

	private void OnDisable()
	{
		ScoreManager.Instance.PlayerScoreReachedThreshold -= PlayerScoreReachedThresholdHandler;
		IronSourceRewardedVideoEvents.onAdRewardedEvent -= ContinueGame;
	}

	private void Start()
	{
		InitGameLevel();
	}

	public void StartGame()
	{
		SpawnManager.Instance.Speed = _startLevelSpeed;
		ScoreManager.Instance.StartCount();
		StateMachine.Instance.ChangeState(StateMachine.Running);
	}

	private void ContinueGame(IronSourcePlacement placement, IronSourceAdInfo adInfo)
	{
		GameUIHandler.Instance.CloseGameOverPanel();
		SpawnManager.Instance.Reset();
		SpawnManager.Instance.Speed = _currentLevelSpeed;
		ScoreManager.Instance.StartCount();
		Player.Instance.Reset();
		StateMachine.Instance.ChangeState(StateMachine.Running);
	}

	private void InitGameLevel()
	{
		Player.Instance.Reset();
		SpawnManager.Instance.Reset();
	}

	private void PlayerScoreReachedThresholdHandler()
	{
		_currentLevelSpeed = SpawnManager.Instance.Speed * 1.25f;
		SpawnManager.Instance.Speed = _currentLevelSpeed;
	}
}