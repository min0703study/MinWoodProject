using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollision : MonoBehaviour
{
	public string AreaName;

	private void OnTriggerEnter2D(Collider2D collided) {
		if(collided.tag == "Player") 
		{
			CinemachineManager.Instance.SwitchCamera(AreaName); 
		}
	}
}
