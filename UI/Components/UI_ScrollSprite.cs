using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ScrollSprite : MonoBehaviour
{
    [SerializeField]
    private float parallaxSpeed;

    private float spriteWidth;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * parallaxSpeed * Time.deltaTime);

        if(transform.position.x < startPos.x - spriteWidth)
        {
            transform.position = startPos;
        }
    }
}
