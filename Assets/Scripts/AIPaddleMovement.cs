using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddleMovement : MonoBehaviour
{
	public Transform ball;
    float topBounds = 11.8f;
    float bottomBounds = -6.5f;

    [Range (0, 1)]
    public float moveSpeed;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		
	}

    public void MoveAIPaddle()
	{
        Vector2 newPos = transform.position;
        newPos.y = Mathf.Clamp(Mathf.Lerp(transform.position.y, ball.position.y, moveSpeed), bottomBounds, topBounds);
        transform.position = newPos;

    }
}
