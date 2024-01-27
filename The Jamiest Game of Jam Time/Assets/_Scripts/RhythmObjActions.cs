using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;

/*
loot boxes
multiple mele weapons
last hit cyoty time
 */

public class RhythmObjActions : MonoBehaviour
{
	public NoteData noteData;
	public GameObject hitModel;
	public Transform root;
	public float reactTime = 2.1f;
	private static List<RhythmObjActions> allActions = new List<RhythmObjActions>();


	public int lane = 0;// 0 = centrer, 1 = left, 2 = right

	// Awake is called only once before Start
	void Awake()
	{
		//Add any initial animation code here!!(NVM DONT DO THAT)
		allActions.Add(this);
	}

	private void OnDestroy()
	{
		allActions.Remove(this);
	}

	private int last;
	//public DateTime timer = DateTime.MinValue;
	public AudioSource clip;
	void NoteUpdate()
	{
		var timings = noteData?.hitTimings;
		if(timings == null) return;


		if(last >= timings.Count) return;

		for(int count = last; count < timings.Count; ++count)
		{
			var time = timings[count] - reactTime;

			if(clip.time - time >= .5f)
				break;

			if(clip.time >= time)
			{
				/*PLACE NOTE LOGIC HERE!!!*/

				switch(noteData.hitTypes[count])
				{
				case HitType.TEST1://melee target
					print("Test1 Triggered");
					StartCoroutine(AnimateMeleeTarget(time + reactTime, reactTime));

					//	StartCoroutine(AnimateMeleeTarget(time, time + reactTime, transform.localPosition));
					break;
				case HitType.TEST2://dodge target
					print("Test2 Triggered");
					StartCoroutine(AnimateEnemyAttack(reactTime + time, reactTime));

					//	StartCoroutine(AnimateNote2(time, time + reactTime, transform.localPosition));
					break;
				case HitType.TEST3://dodge target
					print("Test2 Triggered");
					StartCoroutine(AnimateEnemyAttack(reactTime + time, reactTime));

					//	StartCoroutine(AnimateNote3(time, time + reactTime, transform.localPosition));
					break;
				case HitType.TEST4://dodge target
					print("Test2 Triggered");
					StartCoroutine(AnimateEnemyAttack(reactTime + time, reactTime));

					//	StartCoroutine(AnimateNote4(time, time + reactTime, transform.localPosition));
					break;
				default:
					break;
				}

				last = count + 1;
			}
		}

		if(last >= timings.Count)//automatic cleanup
			StartCoroutine(OnEnemyEnd(reactTime * 3f));
	}

	IEnumerator AnimateMeleeTarget(float timing, float react, Vector3 location = new Vector3())
	{
		var obj = Instantiate(hitModel, transform);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.GetChild(0).localPosition += location;
		obj.GetComponentInChildren<Renderer>().material.color = new Color(1, 0, 0);
		IEnumerator Cleanup()
		{
			yield return new WaitForSeconds(react);
			Destroy(obj);
		}

		var callback = obj.AddComponent<HayYouSlappedSomthing>();

		callback.onObjectHit.AddListener(() => { Destroy(obj); });
		yield return new WaitUntil(() =>
		{
			//Animation Logic Here!!
			if(clip.time >= timing)//on completion
			{
				obj.GetComponentInChildren<Renderer>().material.color = new Color(0, 1, 0);
				StartCoroutine(Cleanup());
				return true;
			}
			// Any other logic

			return false;
		});

		yield break;
	}

	IEnumerator AnimateEnemyAttack(float timing, float react)
	{
		var tmp = GetComponentInChildren<Renderer>().material.color;
		GetComponentInChildren<Renderer>().material.color = new Color(1, 1, 0);

		yield return new WaitUntil(() =>
		{
			//print("Why this no work?");
			//Animation Logic Here!!
			if(clip.time >= timing)//on player can be hit
			{
				GetComponentInChildren<Renderer>().material.color = new Color(0, 0, 1);
				return true;
			}
			if(clip.time >= timing - react * .5)//Player not in danger
			{
				GetComponentInChildren<Renderer>().material.color = new Color(0, 1, 0);
				return false;
			}
			return false;
		});

		yield return new WaitUntil(() =>
		{
			//print("Why this no work?");
			//Animation Logic Here!!
			if(clip.time >= timing + react * .5)//Player not in danger
				return true;

			return false;
		});



		GetComponentInChildren<Renderer>().material.color = tmp;
		yield break;
	}


	IEnumerator AnimateNote1(float timing, float react, Vector3 startLocation = new Vector3())
	{
		
		yield break;
	}
	IEnumerator AnimateNote2(float timing, float react, Vector3 startLocation = new Vector3())
	{
		transform.localPosition = startLocation;

	 	yield break;
	}
	IEnumerator AnimateNote3(float timing, float react, Vector3 startLocation = new Vector3())
	{
		transform.localPosition = startLocation;
 yield break;
	}
	IEnumerator AnimateNote4(float timing, float react, Vector3 startLocation = new Vector3())
	{
		transform.localPosition = startLocation;

		 yield break;
	}


	private IEnumerator OnEnemyEnd(float react)
	{
		yield return new WaitForSeconds(react);
		Destroy(gameObject);
		yield break;
	}

	// Update is called once per frame
	void Update()
	{
		//if(timer == DateTime.MinValue)
		//	timer = DateTime.Now;

		NoteUpdate();
	}
}
