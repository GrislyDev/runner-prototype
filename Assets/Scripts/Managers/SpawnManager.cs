using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public const int DEFAULT_INIT_CHUNKS = 2;
	public static SpawnManager Instance { get; private set; }

	[SerializeField] private GameObject[] _prefabs;
	[SerializeField] private float _speed;

	private List<GameObject> _activeObjects;
	private ObjectPool _objectPool;
	private Coroutine _moveObjectCoroutine;

	public float Speed
	{
		get
		{
			return _speed;
		}
		set
		{
			_speed = value;
			if (_moveObjectCoroutine != null)
				StopCoroutine(_moveObjectCoroutine);
			_moveObjectCoroutine = StartCoroutine(MoveObjectCoroutine());
		}
	}

	#region UNITY_ENGINE
	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		_activeObjects = new List<GameObject>();

		_objectPool = new ObjectPool();
		for (int i = 0; i < _prefabs.Length; i++)
		{
			_objectPool.AddPrefabToPool(_prefabs[i], 2);
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		BorderScanner(collision.gameObject);
	}
	#endregion

	private void MoveObjects()
	{
		if (_activeObjects.Count > 0 && _speed > 0)
		{
			for (int i = 0; i < _activeObjects.Count; i++)
			{
				_activeObjects[i].transform.Translate(Vector3.back * _speed * Time.deltaTime);
			}
		}
	}

	private void BorderScanner(GameObject obj)
	{
		_activeObjects.RemoveAt(0);
		obj.SetActive(false);
		SpawnObject();
	}

	private void SpawnObject()
	{
		// if it will be first chunk, spawn chunk0 at zero pos
		int randomIndex = 0;
		Vector3 pos = Vector3.zero;

		// else take random chunk
		if (_activeObjects.Count > 0)
		{
			pos = _activeObjects[_activeObjects.Count - 1].transform.position + new Vector3(0, 0, 54);
			randomIndex = Random.Range(1, _prefabs.Length);
		}

		var obj = _objectPool.GetPoolObject(_prefabs[randomIndex]);
		obj.transform.position = pos;
		_activeObjects.Add(obj);
	}

	private void StartSpawnManager(int initChunks, float startSpeed)
	{
		for (int i = 0; i < initChunks; ++i)
			SpawnObject();

		_speed = startSpeed;
	}

	public void Reset()
	{
		foreach (var obj in _activeObjects)
			obj.SetActive(false);

		_activeObjects.Clear();

		StartSpawnManager(DEFAULT_INIT_CHUNKS, _speed);
	}

	private IEnumerator MoveObjectCoroutine()
	{
		var delay = new WaitForFixedUpdate();

		while (_speed > 0)
		{
			MoveObjects();
			yield return delay;
		}
	}
}