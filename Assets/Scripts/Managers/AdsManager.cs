using UnityEngine;

public class AdsManager : MonoBehaviour
{
	public static AdsManager Instance { get; private set; }

	private string appKey = "1db06ae05";

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	private void Start()
	{
		IronSource.Agent.validateIntegration();
		IronSource.Agent.init(appKey);
	}

	private void OnEnable()
	{
		IronSourceEvents.onSdkInitializationCompletedEvent += SDKInitialized;
	}

	private void OnDisable()
	{
		IronSourceEvents.onSdkInitializationCompletedEvent -= SDKInitialized;
	}

	private void SDKInitialized() => Debug.Log("IronSource sdk is initialized.");

	private void OnApplicationPause(bool pause)
	{
		IronSource.Agent.onApplicationPause(pause);
	}

	public void LoadRewardedAd()
	{
		IronSource.Agent.loadRewardedVideo();
	}

	public void ShowRewardedAd()
	{
		if (IronSource.Agent.isRewardedVideoAvailable())
		{
			IronSource.Agent.showRewardedVideo();
		}
		else
		{
			Debug.Log("Ads not ready");
		}
	}
}