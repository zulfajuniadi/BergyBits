using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
	public Transform Target;
	void Update()
	{
		Vector3 position = Target.position;
		position.y = 0;
		transform.position = Vector3.Lerp(transform.position, position, Time.unscaledDeltaTime * 2f);
	}
}
