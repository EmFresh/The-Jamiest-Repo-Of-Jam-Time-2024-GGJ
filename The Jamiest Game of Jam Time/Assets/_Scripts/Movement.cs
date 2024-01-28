using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using static UnityEngine.InputSystem.InputAction;
using System;
using UnityEngine.Events;
using System.Reflection;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
	public float jumpForce = 5;
	public float gravity = 5;
	public float movementSpeed = 5;
	public float movementInc = 0.1f;
	public float drag = .5f;

	public AudioSource source=new AudioSource();
	public List<AudioClip> clip = new List<AudioClip>();

	void Update()
	{


		if(moveAction != null) { moveAction.Invoke(); }
		if(jumpAction != null) { jumpAction.Invoke(); }
	}

	UnityAction jumpAction = null;
	public void Jump(CallbackContext ctx)
	{
		var val = transform.up * ctx.action.ReadValue<float>() * jumpForce;

		var body = GetComponent<Rigidbody>();
		var vel = body.velocity;
		var collider = GetComponent<Collider>();

		if(ctx.performed)
			if(Physics.Raycast(transform.position, -transform.up, collider.bounds.extents.y + 0.5f) && body.velocity.y <= 0)
				body.AddForce(val, ForceMode.Impulse);

		if(ctx.canceled)
			body.velocity = new Vector3(vel.x, vel.y * .5f, vel.z);

		jumpAction = () =>
		{
			var body = GetComponent<Rigidbody>();
			if(body.velocity.y <= 0)
				body.AddForce(-transform.up * gravity, ForceMode.Force);

		};
	}


	UnityAction moveAction = null;
	public void Move(CallbackContext ctx)
	{
		var val = ctx.action.ReadValue<Vector2>() * movementSpeed * Time.deltaTime;
		val *= (ctx.performed || ctx.started) ? 1 : 0;
		var body = GetComponent<Rigidbody>();


		moveAction = () =>
		{
			var x = val.x;

			if(Mathf.Abs(x) > 0)
			{
				var collider = GetComponent<Collider>();
				if((Physics.Raycast(transform.position, -Vector3.right, collider.bounds.extents.x + 0.5f) ||
				Physics.Raycast(transform.position, Vector3.right, collider.bounds.extents.x + 0.5f)) &&
				!Physics.Raycast(transform.position, -transform.up, collider.bounds.extents.y + 0.5f)) return;

				body.velocity = new Vector3(x, body.velocity.y, 0);
			}
			else
			{
				body.velocity -= new Vector3(body.velocity.x, 0, 0) * drag;

				if(Math.Abs(body.velocity.magnitude) < .001f)
					body.velocity = new Vector3(0, body.velocity.y, 0);

			}
		};
	}


	private void OnTriggerEnter(Collider other)
	{
		GameObject obj = null;
		if(obj = other.gameObject.GetComponent<RhythmObjMovement>().gameObject)
		{
			System.Random rand = new System.Random();

			source.PlayOneShot(clip[rand.Next(clip.Count)]);
			 
			
			Destroy(obj);
		}
	}
}
