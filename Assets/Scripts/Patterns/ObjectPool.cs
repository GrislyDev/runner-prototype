using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectPool
{
	private Dictionary<string, List<GameObject>> _objects;

    public ObjectPool()
    {
        _objects = new Dictionary<string, List<GameObject>>();

	}

    public void AddPrefabToPool(GameObject prefab, int initSize)
    {
        if (!_objects.ContainsKey(prefab.name))
        {
            var list = new List<GameObject>();
            GameObject obj;

            for (int i = 0; i < initSize; i++)
            {
                obj = GameObject.Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
                obj.SetActive(false);
                list.Add(obj);
            }

            _objects[prefab.name] = list;
        }
    }

    public GameObject GetPoolObject(GameObject prefab)
    {
        if (_objects.ContainsKey(prefab.name))
        {
            GameObject obj;

            var list = _objects[prefab.name];

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].activeInHierarchy)
                {
                    obj = list[i];
                    obj.SetActive(true);
                    return obj;
                }

			}

			obj = GameObject.Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
			list.Add(obj);
			return obj;
		}

        throw new System.NullReferenceException($"Object pool does not has {prefab.name}");
	}
}