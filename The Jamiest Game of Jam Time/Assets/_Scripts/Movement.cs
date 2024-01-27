using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using static UnityEngine.InputSystem.InputAction;
using System;
using UnityEngine.Events;
using Unity.VisualScripting.FullSerializer;
using System.Reflection;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
	public float jumpForce = 5;
	public float movementSpeed = 5;


	// Start is called before the first frame update
	void Awake()
	{

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

	public void Move(CallbackContext ctx)
	{
		var val = ctx.action.ReadValue<Vector2>() * movementSpeed * Time.deltaTime;
		val *= (ctx.performed || ctx.started) ? 1 : 0;


		this.RepeatingCoroutine(nameof(Move), () =>
		{
			transform.position += new Vector3(val.x, 0, val.y);
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
