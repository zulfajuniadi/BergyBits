using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{

	private Platform platform;
	private Transform currentFish;
	private Movment movement;
	private bool canMoveFwd, canMoveBack, canMoveLeft, canMoveRight;
	private float nextCheck;

	// Use this for initialization
	void Start()
	{
		movement = GetComponent<Movment>();
		platform = FindObjectOfType<Platform>();
	}

	// Update is called once per frame
	void Update()
	{
		movement.Vertical = 0;
		movement.Horizontal = 0;
		Vector3 heading = Vector3.zero;
		if (currentFish == null)
		{
			if (platform.Fishes.Count > 0)
			{
				currentFish = platform.Fishes[Random.Range(0, platform.Fishes.Count)];
			}
		}
		else if (
			currentFish.position.x > -10 && currentFish.position.x < 10 &&
			currentFish.position.z > -10 && currentFish.position.z < 10
		)
		{
			heading = (currentFish.position - transform.position).normalized;
			movement.Vertical = heading.z;
			movement.Horizontal = heading.x;
			if (!canMoveFwd)
			{
				movement.Vertical = -1;
			}
			else if (!canMoveBack)
			{
				movement.Vertical = 1;
			}
			if (!canMoveRight)
			{
				movement.Horizontal = -1;
			}
			else if (!canMoveLeft)
			{
				movement.Horizontal = 1;
			}
		}
	}

	private void FixedUpdate()
	{
		if (Time.time > nextCheck)
		{
			nextCheck = Time.time + 0.5f;
			Vector3 pos = transform.position + Vector3.up;
			canMoveFwd = Physics.Raycast(pos + Vector3.forward * 2, Vector3.down, 2);
			canMoveBack = Physics.Raycast(pos + Vector3.back * 2, Vector3.down, 2);
			canMoveLeft = Physics.Raycast(pos + Vector3.left * 2, Vector3.down, 2);
			canMoveRight = Physics.Raycast(pos + Vector3.right * 2, Vector3.down, 2);
		}
	}
}
