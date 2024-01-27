using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
 
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
	public float movementSpeed = 5;
	public float movementInc = 0.1f;
	public float drag = .5f;

	void Update()
	{
		var body = GetComponent<Rigidbody>();
		if(body.velocity.y <= 0)
		{
			var vel = body.velocity; vel.y *= 1.2f;
		}
	}

	public void Jump(CallbackContext ctx)
	{
		if(!ctx.performed) return;

		var val = transform.up * ctx.action.ReadValue<float>() * jumpForce;

		var body = GetComponent<Rigidbody>();
		var collider = GetComponent<Collider>();
		if(Physics.Raycast(transform.position, -transform.up, collider.bounds.extents.y + 0.5f) && body.velocity.y <= 0)
			body.AddForce(val, ForceMode.Impulse);

	}


	bool right = true;
	public void Move(CallbackContext ctx)
	{
		var val = ctx.action.ReadValue<Vector2>() * movementSpeed * Time.deltaTime;
		val *= (ctx.performed || ctx.started) ? 1 : 0;
		var body = GetComponent<Rigidbody>();


		this.RepeatingCoroutine(nameof(Move), () =>
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
		});
	}

}

public static class My_Util
{

	static Dictionary<MonoBehaviour, Dictionary<string, UnityAction>> moves = new Dictionary<MonoBehaviour, Dictionary<string, UnityAction>>();
	static Dictionary<MonoBehaviour, Coroutine> movesCo = new Dictionary<MonoBehaviour, Coroutine>();
	public static void RepeatingCoroutine(this MonoBehaviour src, string UIDName, in UnityAction enumu)
	{
		if(moves.ContainsKey(src))
			moves[src][src.GetHashCode() + UIDName] = enumu;
		else
			moves[src] = new Dictionary<string, UnityAction>(new[] { new KeyValuePair<string, UnityAction>(src.GetHashCode() + UIDName, enumu) });

		IEnumerator Repeat(ICollection<UnityAction> enu)
		{
			//	MonoBehaviour.print("Hi I got here btw IN the thing here first");
			yield return new WaitWhile(() =>
			{
				foreach(var val in moves[src].Values)
					val();
				//		MonoBehaviour.print("Hi I got here btw IN the thing");

				return true;
			});
		}

		if(movesCo.ContainsKey(src))
			src.StopCoroutine(movesCo[src]);

		if(src.isActiveAndEnabled)
			movesCo[src] = src.StartCoroutine(Repeat(moves[src].Values));


		//	MonoBehaviour.print($"Amount of functions: {moves[src].Count}");
	}

	/// <summary>
	/// Adds a value to the end of a list and returns it
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="val"></param>
	/// <returns></returns>
	public static T2 AddNReturnVal<T, T2>(this T list, T2 val) where T : ICollection<T2>
	{
		list.Add(val);
		return list.Last();
	}

	/// <summary>
	/// Adds a value to the end of a list and returns it
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="val"></param>
	/// <returns></returns>
	public static T AddNReturnCollection<T, T2>(this T list, T2 val) where T : ICollection<T2>
	{
		list.Add(val);
		return list;
	}

}
