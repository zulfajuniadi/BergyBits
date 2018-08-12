using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFish : MonoBehaviour
{
	float rotationSpeed;
	float offset;
	float y;

	// Use this for initialization
	void Start()
	{
		offset = Random.Range(-1f, 1f);
		rotationSpeed = Random.Range(25f, 35f);
	}

	// Update is called once per frame
	void Update()
	{
		y -= (Mathf.Sin(Time.timeSinceLevelLoad + offset) + 1f) * rotationSpeed * Time.deltaTime;
		transform.eulerAngles = new Vector3(0, y, 0);
	}
}
