using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour, IScoreManager
{
	public event Action PlayerScoreReachedThreshold;

	public static ScoreManager Instance { get; private set; }

	[SerializeField]
	private TextMeshProUGUI _scoreText;

	private int _score;
	private float _distanceMultiplier = 1;
	private float _bonusMultiplier = 1;
	private HashSet<int> _scoreThresholds = new HashSet<int> { 100, 200, 10000 };

	private Coroutine _scoreCoroutine;
	private Coroutine _bonusCoroutine;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	private void Start()
	{
		_score = 0;
		UpdateScoreText();
	}

	private void IncreaseScore()
	{
		_score += Mathf.RoundToInt(_distanceMultiplier * _bonusMultiplier);
		UpdateScoreText();
	}
	private void UpdateScoreText()
	{
		_scoreText.text = $"Score: {_score:000000}";
	}

	private void CheckScoreThreshold()
	{
		if (_scoreThresholds.Contains(_score))
		{
			PlayerScoreReachedThreshold?.Invoke();
			IncreaseDistanceMultiplier();
		}
	}

	private void IncreaseDistanceMultiplier()
	{
		_distanceMultiplier *= 1.5f;
	}

	public void SetBonusMultiplier(float multiplier, float time)
	{
		if (_bonusCoroutine != null)
		{
			StopCoroutine(_bonusCoroutine);
		}
		_bonusCoroutine = StartCoroutine(BonusMultiplierCoroutine(multiplier, time));
	}

	private IEnumerator ScoreCoroutine()
	{
		var delay = new WaitForSeconds(0.25f);

		while (true)
		{
			yield return delay;

			IncreaseScore();
			CheckScoreThreshold();
		}
	}

	private IEnumerator BonusMultiplierCoroutine(float multiplier, float time)
	{
		float originalMultiplier = _bonusMultiplier;
		_bonusMultiplier = multiplier;
		yield return new WaitForSeconds(time);
		_bonusMultiplier = originalMultiplier;
	}

	public void StartCount()
	{
		_scoreCoroutine = StartCoroutine(ScoreCoroutine());
	}

	public void StopCount()
	{
		if(_scoreCoroutine != null)
			StopCoroutine(_scoreCoroutine);
	}

	public int GetScore() => _score;
}