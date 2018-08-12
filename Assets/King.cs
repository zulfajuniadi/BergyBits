using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
{

	public Transform CurrentPlatform;

	private void OnCollisionEnter(Collision other)
	{
		if (other.collider.CompareTag("Platform"))
		{
			CurrentPlatform = other.collider.transform;
		}
	}
}
