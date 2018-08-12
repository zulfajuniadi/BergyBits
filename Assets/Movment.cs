using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movment : MonoBehaviour
{
	public bool PlayerControlled = false;
	public float Horizontal { get; set; }
	public float Vertical { get; set; }

	[SerializeField]
	float rotationSpeed = 5f;
	[SerializeField]
	float moveSpeed = 3f;
	float nextMovement;
	Animator animator;
	Quaternion offset;
	RaycastHit hit;
	Platform platform;

	public void Start()
	{
		offset = Quaternion.Euler(0, 45, 0);
		animator = GetComponent<Animator>();
		platform = FindObjectOfType<Platform>();
		platform.AddEffector(transform);
	}

	public void Update()
	{
		if (Time.time < nextMovement) return;
		if (transform.position.y < -10)
		{
			AudioManager.PlayFell();
			if (PlayerControlled)
			{
				GameManager.GameOver();
				this.enabled = false;
			}
			else
			{
				Destroy(gameObject);
			}
		}
		bool grounded = CheckGrounded();
		animator.SetBool("IsGrounded", grounded);
		if (PlayerControlled)
		{
			Horizontal = Input.GetAxis("Horizontal");
			Vertical = Input.GetAxis("Vertical");
			if (Input.GetButtonDown("Jump"))
			{
				Attack();
			}
		}
		if (grounded)
			animator.SetBool("IsMoving", Horizontal != 0 || Vertical != 0);
		Vector3 heading = offset * new Vector3(Horizontal, 0, Vertical).normalized;
		float rotateSpeed = rotationSpeed;
		if (PlayerControlled && Input.GetButton("Fire3"))
		{
			heading *= 1.5f;
			rotateSpeed *= 1.5f;
		}
		transform.position += heading * moveSpeed * Time.deltaTime;
		if (heading.magnitude != 0)
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(heading), rotateSpeed * Time.deltaTime);
	}

	private void Attack()
	{
		animator.ResetTrigger("HeadButt");
		animator.SetTrigger("HeadButt");
	}

	private bool CheckGrounded()
	{
		if (Physics.Raycast(transform.position + transform.up, Vector3.down, out hit, 3f))
		{
			animator.SetFloat("NormalAngle", Vector3.Angle(hit.normal, transform.up));
			return true;
		}
		return false;
	}

	public void PlayJump()
	{
		AudioManager.PlayJump();
	}

	private void ImpairMovement()
	{
		animator.SetBool("IsGrounded", false);
		nextMovement += 10f;
	}

	public void PlayHeadButt()
	{
		AudioManager.PlayHeadButt();
		Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up + transform.forward, 2f, LayerMask.GetMask("Units"));
		foreach (var collider in colliders)
		{
			if (collider.transform != transform)
			{
				collider.gameObject.SendMessage("ImpairMovement", SendMessageOptions.DontRequireReceiver);
				var rb = collider.GetComponent<Rigidbody>();
				if (rb)
				{
					Vector3 heading = (collider.transform.position - transform.position).normalized;
					rb.velocity += (heading * 10) + transform.rotation * new Vector3(-5, 8, 0);
					platform.RemoveEffector(collider.transform);
				}
			}
		}
	}

	private void OnDestroy()
	{
		platform.RemoveEffector(transform);
	}
}
