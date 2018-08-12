using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
	public GameObject Marker;
	public GameManager GameManager;
	private Platform platform;
	private bool landed;

	void Start()
	{
		platform = FindObjectOfType<Platform>();
	}

	void Update()
	{
		if (landed && transform.position.y < -3f)
		{
			Destroy(gameObject);
		}
		else if (transform.position.y < -20f)
		{
			Destroy(gameObject);
		}
		if (!landed && transform.position.y > 1)
		{
			landed = true;
			platform.AddFish(transform);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GameManager.AddFish();
			AudioManager.PlayAddFish();
			Destroy(gameObject);
		}
		if (other.CompareTag("Enemy"))
		{
			AudioManager.PlayAddFish();
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (Marker)
		{
			AudioManager.PlayFishFell();
			Destroy(Marker);
		}
	}

	private void OnDestroy()
	{
		if (Marker)
			Destroy(Marker);
		platform.RemoveFish(transform);
	}
}
