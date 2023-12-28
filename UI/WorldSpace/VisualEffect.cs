using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffect : MonoBehaviour
{
	public void SetInfo(Vector2 pos, Transform parent = null)
	{
		transform.position = pos;

		if (parent != null)
		{
			//GetComponent<MeshRenderer>().sortingOrder = 321;
		}
	}

	public void CompletedAnimation()
	{
		ResourceManager.Instance.Destroy(gameObject);

	}
}
