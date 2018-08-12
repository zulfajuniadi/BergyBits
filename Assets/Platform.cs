using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Platform : MonoBehaviour
{
	public float LevelDuration { get; set; }
	public Vector3 PlatformSize { get; set; }
	public float RunTime { get; set; }
	public int Effectors { get { return effectors.Count; } }

	[SerializeField]
	private float platformTilt = 20f;
	[SerializeField]
	private Slider slider;
	private List<Transform> effectors = new List<Transform>();
	public List<Transform> Fishes = new List<Transform>();

	Vector3 endScale;

	void Start()
	{
		LevelDuration = 60f;
		endScale = Vector3.one * 0.1f;
		endScale.y = 1;
	}

	public void AddEffector(Transform trans)
	{
		if (!effectors.Contains(trans))
			effectors.Add(trans);
	}

	public void RemoveEffector(Transform trans)
	{
		if (effectors.Contains(trans))
			effectors.Remove(trans);
	}

	public void AddFish(Transform trans)
	{
		if (!Fishes.Contains(trans))
			Fishes.Add(trans);
	}

	public void RemoveFish(Transform trans)
	{
		if (Fishes.Contains(trans))
			Fishes.Remove(trans);
	}

	void Update()
	{
		Bounds bound = new Bounds();
		foreach (var effector in effectors)
		{
			bound.Encapsulate(effector.position);
		}
		RunTime += Time.deltaTime + 0.2f * Effectors * Time.deltaTime;
		slider.value = (LevelDuration - RunTime) / LevelDuration;
		if (slider.value <= 0)
		{
			GameManager.GameOver();
		}
		else
		{
			transform.position = new Vector3(0, Mathf.Sin(Time.timeSinceLevelLoad) * 0.5f - 2f, 0);
			float rotX = Mathf.Clamp((bound.center.x - transform.position.x) / 7f, -1, 1) * -platformTilt;
			float rotZ = Mathf.Clamp((bound.center.z - transform.position.z) / 7f, -1, 1) * platformTilt;
			Vector3 rotation = new Vector3(rotZ, 0, rotX);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation), 20f * Time.deltaTime);
		}
	}
}
