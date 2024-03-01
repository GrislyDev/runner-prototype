using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIHandler : MonoBehaviour
{
	public static GameUIHandler Instance { get; private set; }

	// Leaderboard
	[SerializeField] private GameObject _scoreElement;
	[SerializeField] private Transform _leaderboardContext;
	public GameObject ScoreElement { get { return _scoreElement; }}
	public Transform LeaderboardContext { get { return _leaderboardContext; } }

	// UI Panel
	[SerializeField] private GameObject _menuPanel;
	[SerializeField] private GameObject _gamePanel;
	[SerializeField] private GameObject _gameOverPanel;
	[SerializeField] private GameObject _leaderboardPanel;

	// Gameover
	[SerializeField] private TextMeshProUGUI _gameOverText;

	private Tween _tween;

	private void Awake()
	{
		if(Instance == null)
			Instance = this;
	}

	public void StartGame()
	{
		_menuPanel.SetActive(false);
		_gamePanel.SetActive(true);

		GameManager.Instance.StartGame();
	}
	public void OpenMenu()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void OpenGameOverPanel()
	{
		_gameOverPanel.SetActive(true);

		_tween.Kill();
		_tween = _gameOverText.DOFade(0, 0);
		_tween = _gameOverText.DOFade(1,2f);
	}

	public void CloseGameOverPanel() => _gameOverPanel.SetActive(false);

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void Logout()
	{
		SceneManager.LoadScene("AuthScene");
	}

	public void OpenLeaderboard()
	{
		FirebaseManager.Instance.LoadLeaderboard();
		_leaderboardPanel.SetActive(true);
	}

	public void CloseLeaderboard()
	{
		_leaderboardPanel.SetActive(false);
	}
}