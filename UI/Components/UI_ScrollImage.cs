using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ScrollImage : MonoBehaviour
{
	[SerializeField]
	private float parallaxSpeed;
	
	private RectTransform rectTransform;
	private float rectWidth;
	
	public bool IsScrolling { get; set; }
	private Vector3 startPos;
	
	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		rectWidth = rectTransform.rect.width;
		startPos = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		if(IsScrolling) 
		{
			float curPosX = transform.position.x;
			float endPosX = rectWidth * -1;
			if(curPosX <= endPosX) 
			{
				transform.position = startPos;
			}
			
			transform.Translate(Vector3.left * (parallaxSpeed * Time.deltaTime));
			
		}
	}
}
