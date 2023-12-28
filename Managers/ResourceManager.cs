using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public class ResourceManager : BaseManager<ResourceManager>
{
	// 실제 로드한 리소스.
	private Dictionary<string, UnityEngine.Object> resources = new Dictionary<string, UnityEngine.Object>();

	protected override void init()
	{
		base.init();
	}

	#region 리소스 로드
	public T Load<T>(string key) where T : Object
	{
		if (resources.TryGetValue(key, out Object resource))
		{
			return resource as T;
		}
		if (resource == null)
		{
			Debug.Log($"Failed to load prefab : {key}");
			return null;
		}

		return null;
	}


	public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
	{
		GameObject prefab = Load<GameObject>($"{key}");
		if (prefab == null)
		{
			Debug.Log($"Failed to Instantiate : {key}");
			return null;
		}

		if (pooling) 
		{
			return PoolManager.Instance.Pop(prefab);
		}

		GameObject go = Object.Instantiate(prefab, parent);
		go.name = prefab.name;
		
		return go;
	}

	public void Destroy(GameObject go)
	{
		if (go == null)
			return;

		if (PoolManager.Instance.Push(go))
			return;

		Object.Destroy(go);
	}


	#endregion

	#region 어드레서블

	public void LoadAllAsync(string label, Action<string, int, int> callback)
	{
		var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Object));
		opHandle.Completed += (op) =>
		{
			int loadCount = 0;
			int totalCount = op.Result.Count;

			foreach (var result in op.Result)
			{
				LoadAsync<Object>(result.PrimaryKey, () =>
					{
						loadCount++;
						callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
					});
			}
		};
	}

	public void LoadAsync<T>(string key, Action callback = null) where T : UnityEngine.Object
	{		
		string loadKey = key;
		if (key.EndsWith(".sprites"))
		{
			var asyncOperation = Addressables.LoadAssetAsync<IList<Sprite>>(loadKey);
			asyncOperation.Completed += (op) =>
			{
				IList<Sprite> sprites = ((IList<Sprite>)op.Result).ToList();
				foreach (Sprite sprite in sprites)
				{
					var oneSpriteKey = $"{key}[{sprite.name}]";
					
					if (resources.TryGetValue(oneSpriteKey, out Object resource))
					{
						continue;
					}
					
					resources.Add(oneSpriteKey, sprite);
				}
				callback?.Invoke();
			};
		}
		else if (key.EndsWith(".sprite"))
		{
			var asyncOperation = Addressables.LoadAssetAsync<Sprite>(loadKey);
			asyncOperation.Completed += (op) =>
			{
				Sprite sprite = op.Result;
				var spriteKey = $"{key}[{sprite.name}]";
				
				if (resources.TryGetValue(spriteKey, out Object resource))
				{
					callback?.Invoke();
					return;
				}
				
				resources.Add(spriteKey, sprite);
				callback?.Invoke();
			};
		}
		else
		{ 
			var asyncOperation = Addressables.LoadAssetAsync<Object>(loadKey);
			asyncOperation.Completed += (op) =>
			{
				if (resources.TryGetValue(key, out Object resource))
				{
					callback?.Invoke();
					return;
				}

				resources.Add(key, op.Result);
				callback?.Invoke();
			};
		}
	}
	
	#endregion
}

